using System;
using sysnum = System.Numerics;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;
using Cysharp.Threading.Tasks;

using Quantum;
using Quantum.Operators;

/// <summary>
/// Any classes outside of QubitCircuit only has access to these QubitSubcircuit's properties and methods.
/// </summary>
public interface IQubitSubcircuit {
    int Count {get;}
    RenderTexture GetRenderTexture(int index, bool isQCIndex);

    IQubitSubcircuit Add(Qubit newQubit);

    /// <summary>
    /// Remove a controllable's subcircuit and set the qubits found in it inactive on the qubit circuit.
    /// </summary>
    void Clear();

    /// <summary>
    /// Remove a specific qubit from the subcircuit and set it inactive on the qubit circuit. 
    /// </summary>
    IQubitSubcircuit RemoveAt(int index, bool isQCIndex);

    void Subscribe(NotifyCollectionChangedEventHandler handler);
    void Unsubscribe(NotifyCollectionChangedEventHandler handler);

    UniTask ApplyUnaryOperator(UnaryOperator unaryOperator, int[] indices, bool isQCIndices);
}

public sealed partial class QubitCircuit {
    /// <summary>
    /// Represents part of the circuit that a controllable has.
    /// Qubit subcircuit indices are the perferred way to access the qubits,
    /// but qubit circuit indices work too.
    /// </summary>
    private sealed class QubitSubcircuit : IQubitSubcircuit {
        // used to determine how to update the composite state
        private static readonly int _unaryOperatorMultiQubitThreshold = 2;  

        private readonly QubitCircuit _qc;
        private readonly Controllable _controllable;
        private readonly ObservableCollection<(int qcIndex, Qubit qubit)> _qubits;  // the higher the index, the more binary value
        public int Count => _qubits.Count;

        // serves to relate a quantum circuit index to the appropiate index in the subcircuit
        private readonly Dictionary<int, int> _QCIndexToQSIndex; 

        // quantum data of the subcircuit
        private Vector<sysnum.Complex> _compositeQuantumState = Vector<sysnum.Complex>.Build.Dense(
            1, sysnum.Complex.One
        );  // act as a scalar

        public QubitSubcircuit(QubitCircuit qubitCircuit, Controllable ctrlable) { 
            _qc = qubitCircuit;
            _controllable = ctrlable; 
            _qubits = new ObservableCollection<(int, Qubit)>();
            _QCIndexToQSIndex = new Dictionary<int, int>();
        }

        #region Qubit Subcircuit Manipulation
        public IQubitSubcircuit Add(Qubit newQubit) {
            int qcIndex = _qc._add(newQubit);

            _qubits.Add((qcIndex, newQubit));
            _QCIndexToQSIndex[qcIndex] = _qubits.Count - 1;

            _compositeQuantumState = _vectorKroneckerProduct(newQubit.QuantumStateVector, _compositeQuantumState);
            return this;
        }

        public void Clear() {
            _qc._clear(_QCIndexToQSIndex.Keys);

            _QCIndexToQSIndex.Clear();
            _qubits.Clear();  // although not needed to get rid of entire subcircuit instance, we need notification of clearance
        }

        public IQubitSubcircuit RemoveAt(int index, bool isQCIndex) {
            (int qsIndex, int qcIndex, _) = _getQubitInfo(index, isQCIndex);
            
            return _removeAt(qsIndex, qcIndex);
        }
        private IQubitSubcircuit _removeAt(int qsIndex, int qcIndex) {
            _QCIndexToQSIndex.Remove(qcIndex);
            // now update the dictionary
            foreach (
                var (circIndex, subcircIndex) in _QCIndexToQSIndex
                    .Where(x => x.Key > qcIndex)  // any circ index that is after the removed index is affected
                    .Select(x => (x.Key, x.Value))
            ) {
                // since the circ index will always stay the same, just update the subcirc index
                _QCIndexToQSIndex[circIndex] = subcircIndex - 1;  // minus 1 because we removed one subcirc element
            }

            _qc._removeAt(qcIndex);

            // now a qubit can be removed from the subcirc, since the dictionary is updated
            _qubits.RemoveAt(qsIndex);

            return this;
        }

        public void Subscribe(NotifyCollectionChangedEventHandler handler) {
            _qubits.CollectionChanged += handler;
        }
        public void Unsubscribe(NotifyCollectionChangedEventHandler handler) {
            _qubits.CollectionChanged -= handler;
        }
        #endregion Qubit Subcircuit Manipulation

        #region Getters and Setters
        public RenderTexture GetRenderTexture(int index, bool isQCIndex) {
            (int qsIndex, int qcIndex, Qubit qubit) = _getQubitInfo(index, isQCIndex);
            return qubit.RenderTexture;
        }
        #endregion Getters and Setters

        #region Quantum Operations
        /// <summary>
        /// Apply a unary operator on qubit(s).
        /// </summary>
        public async UniTask ApplyUnaryOperator(UnaryOperator unaryOperator, int[] indices, bool isQCIndices) {
            int[] qsIndices = await UniTask.WhenAll(from i in indices select _applyOneUnaryOperator(unaryOperator, i, isQCIndices));
            _updateCompositeState(unaryOperator, qsIndices);
        }

        /// <summary>
        /// Apply a unary operator on one qubit. Returns the qubit's subcirc index.
        /// </summary>
        private async UniTask<int> _applyOneUnaryOperator(UnaryOperator unaryOperator, int index, bool isQCIndex) {
            (int qsIndex, _, Qubit qubit) = _getQubitInfo(index, isQCIndex);
            await qubit.ApplyUnaryOperator(unaryOperator);
            return qsIndex;
        }

        /// <summary>
        /// Update the composite state after a unary operator operation.
        /// The amount of qubits determines the most efficient method for updating.
        /// <list type="bullet">
        /// <item>
        /// <description>If less than or equal to threshold, then create a matrix unitary operator.</description>
        /// </item>
        /// <item>
        /// <description>If greater than threshold, then recalculate the entire composite state.</description>
        /// </item>
        /// </list>
        /// </summary>
        private void _updateCompositeState(UnaryOperator unaryOperator, int[] qsIndices) {
            if (_qubits.Count <= _unaryOperatorMultiQubitThreshold) {
                Matrix<sysnum.Complex> unitaryOp = Matrix<sysnum.Complex>.Build.DenseIdentity(1);

                for (int i = _qubits.Count - 1; i >= 0; --i) {
                    Matrix<sysnum.Complex> unaryOp = qsIndices.Contains(i) 
                        ? unaryOperator.Matrix
                        : QuantumFactory.identityOperator.Matrix;

                    unitaryOp = unitaryOp.KroneckerProduct(unaryOp);
                }

                _compositeQuantumState *= unitaryOp;
            }
            else {
                Vector<sysnum.Complex> prevCompVector = Vector<sysnum.Complex>.Build.Dense(1, sysnum.Complex.One);
            
                foreach (var (_, qubit) in _qubits.Select(x => (x.qcIndex, x.qubit))) {
                    prevCompVector = _vectorKroneckerProduct(qubit.QuantumStateVector, prevCompVector);
                }

                _compositeQuantumState = prevCompVector;
            }
        }

        private Vector<sysnum.Complex> _vectorKroneckerProduct(Vector<sysnum.Complex> b, Vector<sysnum.Complex> a) {
            var resList = new List<sysnum.Complex[]>(b.Count);

            foreach (var element in b) {
                resList.Add(a.Multiply(element).ToArray());                
            }

            return Vector<sysnum.Complex>.Build.DenseOfEnumerable(resList.SelectMany(e => e));
        }
        #endregion Quantum Operations
        
        /// <summary>
        /// Get all information about a qubit, which includes itself, its index in the subcirc,
        /// and its index in the circ.
        /// </summary>
        private (int qsIndex, int qcIndex, Qubit qubit) _getQubitInfo(int index, bool isQCIndex) {
            int qsIndex, qcIndex;
            Qubit qubit;

            if (isQCIndex) {
                qsIndex = _QCIndexToQSIndex[index];
                qcIndex = index;
                (_, qubit) = _qubits[qsIndex];
            }
            else {
                qsIndex = index;
                (qcIndex, qubit) = _qubits[qsIndex];
            }

            return (qsIndex, qcIndex, qubit);
        }
    }
}

