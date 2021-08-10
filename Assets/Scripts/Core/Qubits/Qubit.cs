using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Quantum;
using Quantum.Operators;

/// <summary>
/// Represents a quantum state on a Bloch sphere.
/// </summary>
public sealed class Qubit : MonoBehaviour
{
    [SerializeField] private BasisQuantumState _initialState;  // default is the ground state
    private QuantumState _quantumState;
    // temp; uses position to determine representation on bloch sphere
    // todo; use rotation to determine representation on bloch sphere
    private GameObject _quantumStateIndicator;  

    private void Awake() {
        _quantumState = QuantumFactory.MakeQuantumState(_initialState);
        _quantumStateIndicator = transform.GetChild(0).gameObject;  // index 0 is the indicator
    }

    /// <summary>
    /// After applying the unary operator, update its position. This does not notify Controllable of any changes.
    /// </summary>
    public void ApplyUnaryOperator(UnaryOperator unaryOperator) {
        _quantumState.ApplyUnaryOperator(unaryOperator);
        Vector3 unityPos = QuantumFactory.GetUnityPosition(_quantumState);
        _quantumStateIndicator.transform.position += unityPos;
    }
}
