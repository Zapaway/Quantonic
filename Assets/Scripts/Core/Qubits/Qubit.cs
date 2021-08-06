  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Quantum;

public class Qubit : MonoBehaviour
{
    [SerializeField] private BasisQuantumState _initalState;
    private QuantumState _quantumState;

    private void Awake() {
        _quantumState = QuantumFactory.MakeQuantumState(_initalState);
    }

    // Create a qubit state machine for this, so that the qubit state can be determined
    // Create a way to update the gameobject to represent the quantumstate
}
