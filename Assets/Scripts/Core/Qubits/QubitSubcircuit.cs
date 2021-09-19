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

    /// <summary>
    /// Apply a unary operator on qubit(s).
    /// </summary>
    UniTask ApplyUnaryOperator(UnaryOperator unaryOperator, int[] indices, bool isQCIndices);
    /// <summary>
    /// Apply a binary operator on one qubit pair. 
    /// If the binary operator is a controlled operator, 
    /// the zero index is the control and the one index is the target.
    /// </summary>
    UniTask ApplyBinaryOperator(BinaryOperator binaryOperator, int[] indexPair, bool isQCPair);
}

public sealed partial class QubitCircuit {
    /// <summary>
    /// Represents part of the circuit that a controllable has.
    /// Qubit subcircuit indices are the perferred way to access the qubits,
    /// but qubit circuit indices work too.
    /// </summary>
    private sealed class QubitSubcircuit : IQubitSubcircuit {
        private readonly QubitCircuit _qc;
        private readonly Controllable _controllable;
        private readonly ObservableCollection<(int qcIndex, Qubit qubit)> _qubits;  // the higher the index, the more binary value
        public int Count => _qubits.Count;

        // serves to relate a quantum circuit index to the appropiate index in the subcircuit
        private readonly Dictionary<int, int> _QCIndexToQSIndex; 

        // quantum data of the subcircuit (is loosely connected to the qubits)
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
        public async UniTask ApplyUnaryOperator(UnaryOperator unaryOperator, int[] indices, bool areQCIndices) {
            // apply the unary operator to the qubit(s) individually
            int[] qsIndices = await UniTask.WhenAll(from i in indices select _applyOneUnaryOperator(unaryOperator, i, areQCIndices));

            // update the composite state after the unary operators
            _updateCompositeState(unaryOperator, qsIndices);
        }
        public async UniTask ApplyBinaryOperator(BinaryOperator binaryOperator, int[] indexPair, bool isQCPair) {            
            // do note that if the binary operator isn't a controlled, then order does not matter
            (int controlQSIndex, _, Qubit controlQubit) = _getQubitInfo(indexPair[0], isQCPair);
            (int targetQSIndex, _, Qubit targetQubit) = _getQubitInfo(indexPair[1], isQCPair);

            // update the composite state 
            var ctrlledOp = binaryOperator as IControlledOperator<BinaryOperator>;
            if (ctrlledOp != null) {
                bool didChange = await _updateCompositeState(ctrlledOp, controlQSIndex, targetQSIndex);
                if (didChange) {
                    await _applyOneUnaryOperator(ctrlledOp.TargetUnaryOperator, targetQSIndex, isQCIndex: false);
                }
            } 
            else {
                _updateCompositeState(binaryOperator, controlQSIndex, targetQSIndex);
            }

            await UniTask.Yield();
        }
        /// <summary>
        /// Apply a unary operator on one qubit. Returns the qubit's subcirc index.
        /// </summary>
        private async UniTask<int> _applyOneUnaryOperator(UnaryOperator unaryOperator, int index, bool isQCIndex) {
            (int qsIndex, _, Qubit qubit) = _getQubitInfo(index, isQCIndex);
            Debug.Log(qubit.name);
            await qubit.ApplyUnaryOperator(unaryOperator);
            return qsIndex;
        }

        /// <summary>
        /// Create the unitary operator using little endian ordering.
        /// </summary>
        /// <param name="operatorMatrices">
        /// <para>Matrices that are ordered from highest to lowest qubit subcirc value.</para>
        /// (e.x. operator on qs 2, operator on qs 1, operator on qs 0)
        /// </param>
        private Matrix<sysnum.Complex> _createUnitaryGate(Matrix<sysnum.Complex>[] operatorMatrices) {
            Matrix<sysnum.Complex> unitaryOp = Matrix<sysnum.Complex>.Build.DenseIdentity(1);

            foreach (var matrix in operatorMatrices) {
                unitaryOp = unitaryOp.KroneckerProduct(matrix);
            }

            return unitaryOp;
        }
        /// <summary>
        /// Update the composite state after a unary operator operation.
        /// </summary>
        private void _updateCompositeState(UnaryOperator unaryOperator, int[] qsIndices) {
            Matrix<sysnum.Complex>[] operatorMatrices = (
                from i in Enumerable.Range(0, _qubits.Count).Reverse() 
                select qsIndices.Contains(i) 
                    ? unaryOperator.Matrix 
                    : QuantumFactory.identityOperator.Matrix
            ).ToArray();

            _compositeQuantumState *= _createUnitaryGate(operatorMatrices);
        }
        /// <summary>
        /// Update the composite state after a binary controlled operator operation.
        /// </summary>
        /// <returns>Did the composite state change.</returns>
        private async UniTask<bool> _updateCompositeState(
            IControlledOperator<BinaryOperator> binaryCtrlOperator, int controlQSIndex, int targetQSIndex
        ) {
            if (controlQSIndex == targetQSIndex) {
                throw new ArgumentException("Control index and target index may not equal each other.");
            }

            async UniTask<Matrix<sysnum.Complex>> _createDenseMatrixOperator(
                Matrix<sysnum.Complex> densityMatrix, 
                Matrix<sysnum.Complex> specialOperatorMatrix
            ) {
                await UniTask.Yield();

                // used to determine order of operators 
                bool controlIndexLessThanTargetIndex = controlQSIndex < targetQSIndex;  

                // determine amount of matrices needed to make binary controlled unitary
                int num_of_identities = Math.Abs(controlQSIndex - targetQSIndex) - 1;

                // create the list of operators
                List<Matrix<sysnum.Complex>> operators = new List<Matrix<sysnum.Complex>>(num_of_identities + 2);

                // add operators left to the density matrix
                //  special operator
                operators.Add(specialOperatorMatrix);  
                //  identities
                for (int _ = 0; _ < num_of_identities; ++_) operators.Add(QuantumFactory.identityOperator.Matrix);
                //  density matrix
                operators.Add(densityMatrix);  

                // it should be operators right to the density matrix if not less than
                if (!controlIndexLessThanTargetIndex) operators.Reverse();

                return _createUnitaryGate(operators.ToArray());
            }

            // create binary controlled unitary
            Matrix<sysnum.Complex> binaryControlledUnitary;
            {
                var (zeroUnitary, oneUnitary) = await UniTask.WhenAll(
                    _createDenseMatrixOperator(  // zero
                        QuantumFactory.groundStateDensityMatrix,
                        QuantumFactory.identityOperator.Matrix
                    ),
                    _createDenseMatrixOperator(  // one
                        QuantumFactory.excitedStateDensityMatrix,
                        binaryCtrlOperator.TargetUnaryOperator.Matrix
                    )
                );

                binaryControlledUnitary = zeroUnitary + oneUnitary;
            }

            // create whole unitary to cover entire composite 
            Matrix<sysnum.Complex> compositeStateUnitary;
            {
                List<Matrix<sysnum.Complex>> operatorMatrices = new List<Matrix<sysnum.Complex>>(_qubits.Count - 2);
                for (int i = _qubits.Count() - 1; i >= 0; --i) {
                    if (i == controlQSIndex || i == targetQSIndex) {
                        operatorMatrices.Add(binaryControlledUnitary);

                        // get to the other end of binary operator and will skip over it after this iteration ends
                        i -= Mathf.Abs(controlQSIndex - targetQSIndex);  
                    } 
                    else operatorMatrices.Add(QuantumFactory.identityOperator.Matrix);
                }

                compositeStateUnitary = _createUnitaryGate(operatorMatrices.ToArray());
            }

            Vector<sysnum.Complex> newCQS = _compositeQuantumState *= compositeStateUnitary;
            bool didChange = newCQS != _compositeQuantumState;
            _compositeQuantumState = newCQS;

            return didChange;
        }
        /// <summary>
        /// Update the composite state after a binary operator operation. 
        /// </summary>
        private bool _updateCompositeState(BinaryOperator binaryOperator, int qsIndexA, int qsIndexB) {
            throw new System.NotImplementedException();
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

