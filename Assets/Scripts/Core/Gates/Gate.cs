using System.Collections.ObjectModel;
using UnityEngine;

using Quantum;
using Quantum.Operators;

/// <summary>
/// All gate GameObjects inherit from this.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public abstract class Gate<T> : MonoBehaviour where T : QuantumOperator {
    protected T _operator;

    /// <value>
    /// How many qubits needed to use this gate. 
    /// </value>
    public abstract int Capacity {get;} 

    /// <summary>
    /// Apply the operator onto the QuantumState(s).
    /// </summary>
    protected abstract void _apply(ReadOnlyCollection<QuantumState> quantumStates);
}