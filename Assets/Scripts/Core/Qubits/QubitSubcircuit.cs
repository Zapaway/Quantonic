using System;
using sysnum = System.Numerics;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

using Quantum;

/// <summary>
/// Any classes outside of QubitCircuit only has access to these QubitSubcircuit's properties and methods.
/// </summary>
public interface IQubitSubcircuit {
    IQubitSubcircuit Add(Qubit newQubit);

    /// <summary>
    /// Remove a controllable's subcircuit and set the qubits found in it inactive on the qubit circuit.
    /// </summary>
    void Clear();

    /// <summary>
    /// Remove a specific qubit from the subcircuit and set it inactive on the qubit circuit. 
    /// </summary>
    IQubitSubcircuit RemoveAt(int index, bool isQCIndex);
}

public sealed partial class QubitCircuit {
    /// <summary>
    /// Represents part of the circuit that a controllable has.
    /// </summary>
    private sealed class QubitSubcircuit : IQubitSubcircuit {
        private readonly QubitCircuit _qc;
        private readonly Controllable _controllable;
        private readonly ObservableCollection<(int qcIndex, Qubit qubit)> _qubits;

        // serves to relate a quantum circuit index to the appropiate index in the subcircuit
        private readonly Dictionary<int, int> _QCIndexToQSIndex; 

        public QubitSubcircuit(QubitCircuit qubitCircuit, Controllable ctrlable) { 
            _qc = qubitCircuit;
            _controllable = ctrlable; 
            _qubits = new ObservableCollection<(int, Qubit)>();
        }

        #region Qubit Subcircuit Manipulation
        public IQubitSubcircuit Add(Qubit newQubit) {
            int qcIndex = _qc._add(newQubit);

            _qubits.Add((qcIndex, newQubit));
            _QCIndexToQSIndex[qcIndex] = _qubits.Count - 1;

            return this;
        }

        public void Clear() {
            _qc._clear(_QCIndexToQSIndex.Keys);

            _QCIndexToQSIndex.Clear();
            _qubits.Clear();  // although not needed to get rid of entire subcircuit instance, we need notification of clearance
        }

        public IQubitSubcircuit RemoveAt(int index, bool isQCIndex) {
            int qsIndex, qcIndex;
            if (isQCIndex) {
                qsIndex = _QCIndexToQSIndex[index];
                qcIndex = index;
            }
            else {
                qsIndex = index;
                (qcIndex, _) = _qubits[qsIndex];
            }
            
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
        #endregion Qubit Subcircuit Manipulation
    }
}

