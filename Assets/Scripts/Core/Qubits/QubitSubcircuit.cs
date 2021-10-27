using System;
using sysnum = System.Numerics;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    Vector<sysnum.Complex>[] Vectors {get;}
    (int controlQSIndex, int targetQSIndex)[] EntanglementInfo {get;}

    RenderTexture GetRenderTexture(int index, bool isQCIndex);
    (string descString, double ground, double excited) GetQubitInfoUnsafe(int index, bool isQCIndex);
    bool IsQubitTarget(int index, bool isQCIndex);
    bool IsQubitControl(int index, bool isQCIndex);
    bool IsQubitEntangled(int index, bool isQCIndex);
    (int controlQSIndex, int targetQSIndex) GetEntangledQSIndexPair(int index, bool isQCIndex);

    /// <summary>
    /// Add an available qubit onto the qubit subcircuit.
    /// </summary>
    (IQubitSubcircuit subcirc, Qubit qubit) Add();
    /// <summary>
    /// "Add" a qubit to the qubit subcirc by explicitly enabling a qubit with the circuit index.
    /// </summary>
    (IQubitSubcircuit subcirc, Qubit qubit) Add(int qcIndex);
    /// <summary>
    /// Add an available qubit with the specified quantum state onto the qubit subcircuit.
    /// </summary>
    (IQubitSubcircuit subcirc, Qubit qubit) Add(QuantumState state);

    /// <summary>
    /// Remove a controllable's subcircuit and set the qubits found in it inactive on the qubit circuit.
    /// </summary>
    void Clear();

    /// <summary>
    /// Remove a specific qubit from the subcircuit and set it inactive on the qubit circuit.
    /// </summary>
    (IQubitSubcircuit subcirc, int avalQCIndex) RemoveAt(int index, bool isQCIndex);

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
    /// <summary>
    /// Force entanglement upon two qubits. Not recommended to use unless if you are loading in quantum states.
    /// </summary>
    void ForceEntanglement(int controlQSIndex, int targetQSIndex);
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
        private readonly ObservableCollection<(int qcIndex, Qubit qubit)> _qubits;  // the higher the qs index, the more binary value
        public int Count => _qubits.Count;
        public Vector<sysnum.Complex>[] Vectors => (from qubitInfo in _qubits select qubitInfo.qubit.QuantumStateVector.Clone()).ToArray();
        public (int controlQSIndex, int targetQSIndex)[] EntanglementInfo => (
            from i in Enumerable.Range(0, _qubits.Count) select GetEntangledQSIndexPair(i, isQCIndex: false)
        ).ToArray();

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
        public (IQubitSubcircuit, Qubit) Add() {
            var (avalQubit, qcIndex) = _qc._getAval();
            _add(qcIndex, avalQubit);

            return (this, avalQubit);
        }
        public (IQubitSubcircuit, Qubit) Add(int qcIndex) {
            Qubit qubit = _qc._enable(qcIndex);
            _add(qcIndex, qubit);

            return (this, qubit);
        }
        public (IQubitSubcircuit, Qubit) Add(QuantumState state) {
            var (_, avalQubit) = Add();
            avalQubit.SetQuantumState(state);

            return (this, avalQubit);
        }
        private void _add(int qcIndex, Qubit qubit) {
            if (qcIndex != -1) {
                _qubits.Add((qcIndex, qubit));
                _QCIndexToQSIndex[qcIndex] = _qubits.Count - 1;

                _compositeQuantumState = _vectorKroneckerProduct(qubit.QuantumStateVector, _compositeQuantumState);
            } else throw new NotImplementedException();
        }

        public void Clear() {
            _qc._clear(_QCIndexToQSIndex.Keys);

            _QCIndexToQSIndex.Clear();
            _compositeQuantumState = Vector<sysnum.Complex>.Build.Dense(
                1, sysnum.Complex.One
            );  // act as a scalar
            _qubits.Clear();  // although not needed to get rid of entire subcircuit instance, we need notification of clearance
        }

        public (IQubitSubcircuit, int) RemoveAt(int index, bool isQCIndex) {
            (int qsIndex, int qcIndex, Qubit qubit) = _getQubitInfo(index, isQCIndex);
            return (this, _removeAt(qsIndex, qcIndex, qubit));
        }
        private int _removeAt(int qsIndex, int qcIndex, Qubit qubit) {
            _QCIndexToQSIndex.Remove(qcIndex);
            // now update the dictionary
            foreach (
                var (circIndex, subcircIndex) in _QCIndexToQSIndex.ToArray()
                    .Where(x => x.Key > qcIndex)  // any circ index after the index that is about to be removed is affected
                    .Select(x => (x.Key, x.Value))
            ) {
                int newSubcircIndex = subcircIndex - 1;

                // since the circ index will always stay the same, just update the subcirc index
                _QCIndexToQSIndex[circIndex] = newSubcircIndex;  // minus 1 because we removed one subcirc element
            }

            _qc._removeAt(qcIndex);

            // now a qubit can be removed from the subcirc, since the dictionary and the qubits are updated
            _qubits.RemoveAt(qsIndex);

            _compositeQuantumState = _recalculateCompositeState();
            return qcIndex;
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
            (_, _, Qubit qubit) = _getQubitInfo(index, isQCIndex);
            return qubit.RenderTexture;
        }

        public (string descString, double ground, double excited) GetQubitInfoUnsafe(int index, bool isQCIndex) {
            (_, _, Qubit qubit) = _getQubitInfo(index, isQCIndex);
            string descString = qubit.DescriptionString;

            if (qubit.Description == QuantumStateDescription.EntangledControl) 
                descString += $" with [{_getQubitInfo(qubit.EntangledPair.target).qsIndex}]";
            else if (qubit.Description == QuantumStateDescription.EntangledTarget)
                descString += $" with [{_getQubitInfo(qubit.EntangledPair.control).qsIndex}]";

            return (descString, qubit.Probabilities.ground, qubit.Probabilities.excited);
        }

        public bool IsQubitTarget(int index, bool isQCIndex) {
            (_, _, Qubit qubit) = _getQubitInfo(index, isQCIndex);
            return qubit.Description == QuantumStateDescription.EntangledTarget;
        }
        public bool IsQubitControl(int index, bool isQCIndex) {
            (_, _, Qubit qubit) = _getQubitInfo(index, isQCIndex);
            return qubit.Description == QuantumStateDescription.EntangledControl;
        }
        public bool IsQubitEntangled(int index, bool isQCIndex) {
            (_, _, Qubit qubit) = _getQubitInfo(index, isQCIndex);
            return qubit.EntangledPair != (null, null);        
        }

        public (int controlQSIndex, int targetQSIndex) GetEntangledQSIndexPair(int index, bool isQCIndex) {
            (_, _, Qubit qubit) = _getQubitInfo(index, isQCIndex);
            int controlQSIndex = -1, targetQSIndex = -1;

            if (qubit.EntangledPair != (null, null)) {
                if (qubit.Description == QuantumStateDescription.EntangledControl) {
                    controlQSIndex = index;
                    targetQSIndex = _getQubitInfo(qubit.EntangledPair.target).qsIndex;
                }
                else {
                    controlQSIndex = _getQubitInfo(qubit.EntangledPair.control).qsIndex;
                    targetQSIndex = index;
                }
            }

            return (controlQSIndex, targetQSIndex);
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
            await UniTask.Yield();     

            // do note that if the binary operator isn't a controlled, then order does not matter
            (int controlQSIndex, _, Qubit controlQubit) = _getQubitInfo(indexPair[0], isQCPair);
            (int targetQSIndex, _, Qubit targetQubit) = _getQubitInfo(indexPair[1], isQCPair);

            // update the composite state 
            var ctrlledOp = binaryOperator as IControlledOperator<BinaryOperator>;
            if (ctrlledOp != null) {
                bool didChange = await _updateCompositeState(ctrlledOp, controlQSIndex, targetQSIndex);
                if (didChange) {
                    await _applyOneUnaryOperator(ctrlledOp.TargetUnaryOperator, targetQSIndex, isQCIndex: false);

                    if (controlQubit.Description == QuantumStateDescription.EntangledControl) {
                        controlQubit.AttemptRevertToRegular();
                    } else controlQubit.AttemptTransformToControl(targetQubit);
                }
            } 
            else {
                _updateCompositeState(binaryOperator, controlQSIndex, targetQSIndex);
            }
        }
        public void ForceEntanglement(int controlQSIndex, int targetQSIndex) {
            ( _, _, Qubit controlQubit) = _getQubitInfo(controlQSIndex, isQCIndex: false);
            (_, _, Qubit targetQubit) = _getQubitInfo(targetQSIndex, isQCIndex: false);

            controlQubit.AttemptTransformToControl(targetQubit);
        }
        /// <summary>
        /// Apply a unary operator on one qubit. Returns the qubit's subcirc index.
        /// Note that it does not care if the affected qubit is entangled.
        /// </summary>
        private async UniTask<int> _applyOneUnaryOperator(UnaryOperator unaryOperator, int index, bool isQCIndex) {
            (int qsIndex, _, Qubit qubit) = _getQubitInfo(index, isQCIndex);
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
            await UniTask.Yield();

            async UniTask<Matrix<sysnum.Complex>> _createDenseMatrixOperator(
                Matrix<sysnum.Complex> densityMatrix, 
                Matrix<sysnum.Complex> specialOperatorMatrix
            ) {
                await UniTask.Yield();

                // used to determine order of operators 
                bool controlIndexLessThanTargetIndex = controlQSIndex < targetQSIndex;  

                // determine amount of matrices needed to make binary controlled unitary
                int numOfIdentities = Math.Abs(controlQSIndex - targetQSIndex) - 1;

                // create the list of operators
                List<Matrix<sysnum.Complex>> operators = new List<Matrix<sysnum.Complex>>(numOfIdentities + 2);

                // add operators left to the density matrix
                //  special operator
                operators.Add(specialOperatorMatrix);  
                //  identities
                for (int _ = 0; _ < numOfIdentities; ++_) operators.Add(QuantumFactory.identityOperator.Matrix);
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

            Vector<sysnum.Complex> newCQS = _compositeQuantumState * compositeStateUnitary;
            bool didChange = !newCQS.Equals(_compositeQuantumState);
            _compositeQuantumState = newCQS;

            return didChange;
        }
        /// <summary>
        /// Update the composite state after a binary operator operation. 
        /// </summary>
        private bool _updateCompositeState(BinaryOperator binaryOperator, int qsIndexA, int qsIndexB) {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Recalculate composite state with the current qubits in the list.
        /// </summary>
        private Vector<sysnum.Complex> _recalculateCompositeState() {
            Vector<sysnum.Complex> res = Vector<sysnum.Complex>.Build.Dense(1, sysnum.Complex.One);

            for (int i = _qubits.Count() - 1; i >= 0; --i) {
                var (_, qubit) = _qubits[i];
                res = _vectorKroneckerProduct(res, qubit.QuantumStateVector);
            }

            return res;
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
        /// <summary>
        /// Find all indices about the given qubit. 
        /// </summary>
        private (int qsIndex, int qcIndex) _getQubitInfo(Qubit qubit) {
            for (int qsIndex = 0; qsIndex < _qubits.Count; ++qsIndex) {
                var info = _qubits[qsIndex];

                if (info.qubit == qubit) {
                    return (qsIndex, info.qcIndex);
                }
            }

            return (-1, -1);  // fail case
        }
    }
}

