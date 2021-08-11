using System.Collections.Generic;
using UnityEngine;

using Managers;
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
    // all qubits that the controllable has
    // - will always at least one unless specified
    private List<Qubit> _qubits;   

    protected virtual void Awake() {
        _qubits = new List<Qubit>();
    }

    #region Qubit List Manipulation
    /// <summary>
    /// Checks if the GameObject is a qubit and returns it. If true, it adds it onto the list.
    /// </summary>
    protected bool _addQubitSafe(GameObject possibleQubit) {
        bool isQubit = possibleQubit.CompareTag("Qubit");
        if (isQubit) {
            _qubits.Add(possibleQubit.GetComponent<Qubit>());
        }
        return isQubit;
    }
    /// <summary>
    /// Just adds the GameObject into the list without checking. Can result in null values being put.
    /// </summary>
    protected void _addQubitUnsafe(GameObject possibleQubit) {
        _qubits.Add(possibleQubit.GetComponent<Qubit>());
    }
    /// <summary>
    /// Creates a qubit from prefab and adds it onto the list.
    /// </summary>
    protected void _addQubitFromPrefab(float xOffset) {
        _qubits.Add(SpawnManager.MakeQubit(xOffset));
    }
    #endregion Qubit List Manipulation
    
    #region Select Qubits
    /// <summary>
    /// Ask the controllable what single qubit to use.
    /// (For now, use the first qubit in the controllable)
    /// </summary>
    public int AskForSingleQubitIndex() {
        return 0;
    }
    #endregion Select Qubits

    // using these methods will automatically notify the controllable to check its qubit state
    #region Applying Methods
    public void ApplyUnaryOperator(UnaryOperator unaryOperator, int index) {
        _qubits[index].ApplyUnaryOperator(unaryOperator);

        // TODO: update qubitstatemachine
    }
    #endregion Applying Methods
}