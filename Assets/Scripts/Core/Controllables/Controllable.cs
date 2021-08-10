using System.Collections.Generic;
using UnityEngine;

using Quantum.Operators;

/*
TODO: 
    - add qubitstatemachine 
*/

/// <summary>
/// Any object that can be controlled with input derives from this base class.
/// </summary>
[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public abstract class Controllable : MonoBehaviour
{
    [SerializeField] protected GameObject _qubitPrefab;
    protected List<Qubit> _qubits;

    protected virtual void Awake() {
        _qubits = new List<Qubit>();
    }

    protected void _addQubit(GameObject possibleQubit) {
        if (possibleQubit.CompareTag("Qubit")) {
            _qubits.Add(possibleQubit.GetComponent<Qubit>());
        }
    }
    protected void _addQubitFromPrefab(float xOffset) {
        GameObject qubit = Instantiate(
            _qubitPrefab, 
            Vector3.zero + new Vector3(xOffset, 0, 0), 
            _qubitPrefab.transform.rotation
        );
        _qubits.Add(qubit.GetComponent<Qubit>());
    }
    
    /// <summary>
    /// Ask the controllable what single qubit to use.
    /// (For now, use the first qubit in the controllable)
    /// </summary>
    public int AskForSingleQubitIndex() {
        return 0;
    }

    // using these methods will automatically notify the controllable to check its qubit state
    #region Applying Methods
    public void ApplyUnaryOperator(UnaryOperator unaryOperator, int index) {
        _qubits[index].ApplyUnaryOperator(unaryOperator);

        // TODO: update qubitstatemachine
    }
    #endregion Applying Methods
}