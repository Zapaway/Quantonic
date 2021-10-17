using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

using Managers;

/// <summary>
/// Extends functionality of quantum states by allowing interactions between them via qubits.
/// Every control manager has this. Each controllable share different parts of the circuit
/// called subcircuits. This is also the object pool for the qubits.
/// </summary>
public sealed partial class QubitCircuit
{   
    #region Fields/Properties
    private const int _defaultCapacity = 20;
    private const int _spacing = 10;
    private const int _leftOffset = 100;

    private Controllable CurrentControllable => StageControlManager.Instance.CurrentControllable;

    // there should be no removing of any qubit, only setting it inactive
    private List<Qubit> _allQubits;  
    public int Count => _allQubits.Count;

    // use a controllable's ID to access its subcircuit (their qubits)
    private readonly Dictionary<int, QubitSubcircuit> _allSubcircuits;

    // allows the same handler to be used across any current controllable's subcirc
    private NotifyCollectionChangedEventHandler _subcircuitCollectionChangedHandler;

    #endregion Fields/Properties

    public QubitCircuit() {
        _allSubcircuits = new Dictionary<int, QubitSubcircuit>();
    }
    public void InitQubitCircuit(StageControlManager controlManager) {
        // set up the events
        StageUIManager.Instance.SetQQVRenderTextures();

        // pool the qubits
        _allQubits = SpawnManager.Instance.MakeQubits(_defaultCapacity, _spacing, _leftOffset);

        controlManager.OnCurrentControllableChanged += (object sender, OnCurrentControllableChangedEventArgs e) => {
            Controllable oldCtrllable = e.OldValue, newCtrllable = e.NewValue;
            oldCtrllable?.UnsubscribeToSubcircuitCollection(_subcircuitCollectionChangedHandler);
            newCtrllable?.SubscribeToSubcircuitCollection(_subcircuitCollectionChangedHandler);
        };
    }

    /// <summary>
    /// Get a qubit subcircuit. Controllable value must not be null.
    /// </summary>
    public IQubitSubcircuit GetQubitSubcircuit(Controllable controllable) {
        return _allSubcircuits[controllable.GetHashCode()];
    }
    /// <summary>
    /// Get a qubit subcircuit. If it cannot be found, it will create a new subcirc and return that.
    /// Controllable value must not be null.
    /// </summary>
    public IQubitSubcircuit GetOrCreateQubitSubcircuit(Controllable controllable) {
        if (_allSubcircuits.ContainsKey(controllable.GetHashCode())) {
            return GetQubitSubcircuit(controllable);
        }

        QubitSubcircuit subcirc = new QubitSubcircuit(this, controllable);
        _allSubcircuits[controllable.GetHashCode()] = subcirc;
        return subcirc;
    }

    public void AddSubcircuitHandler(NotifyCollectionChangedEventHandler handler) {
        _subcircuitCollectionChangedHandler += handler;
    }
    public void RemoveSubcircuitHandler(NotifyCollectionChangedEventHandler handler) {
        _subcircuitCollectionChangedHandler -= handler;
    }

    #region Qubit Circuit Manipulation
    /// <summary>
    /// Transfer selected qubits using qcIndices from one controllable to another.
    /// Assume that the new controllable does not have a subcirc.
    /// </summary>
    public void TransferQubits(Controllable oldCtrl, Controllable newCtrl, int[] qsIndices) {
        IQubitSubcircuit oldSubcirc = GetQubitSubcircuit(oldCtrl);
        IQubitSubcircuit newSubcirc = GetOrCreateQubitSubcircuit(newCtrl);

        foreach (int i in qsIndices) {
            (_, int avalQCIndex) = oldSubcirc.RemoveAt(i, isQCIndex: false);
            newSubcirc.Add(avalQCIndex);
        }
    }

    /// <summary>
    /// Remove all subcircuits and set every qubit inactive.
    /// Note that there is no notification of each subcircuit being cleared.
    /// </summary>
    public void ClearAllQubits() {
        foreach (var qubit in _allQubits) {
            _setActiveQubit(qubit, false);
        }

        _allSubcircuits.Clear();
    }
    #endregion Qubit Circuit Manipulation

    #region Qubit Subcircuit Helpers
    /// <summary>
    /// Get an available qubit and return it and its qcIndex. 
    /// </summary>
    private (Qubit, int) _getAval() {
        int qcIndex = -1;

        Qubit qubit = _allQubits.FirstOrDefault(qubit => {
            qcIndex++;
            return !qubit.gameObject.activeSelf;
        });
        if (qubit == null) qcIndex = -1;
        else _setActiveQubit(qubit, true);

        return (qubit, qcIndex);
    }
    /// <summary>
    /// Get a qubit that is not available but not in use currently.
    /// </summary>
    private int _getNotUsed() {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Forcibly "add" a qubit to the circuit by enabling it.
    /// </summary>
    private Qubit _enable(int qcIndex) {
        Qubit qubit = _allQubits[qcIndex];
        _setActiveQubit(qubit, true);

        return qubit;
    }

    private void _clear(Dictionary<int, int>.KeyCollection qcIndices) {
        foreach (int qcIndex in qcIndices) {
            _setActiveQubit(_allQubits[qcIndex], false);
        }
    }

    private void _removeAt(int qcIndex) {
        _setActiveQubit(_allQubits[qcIndex], false);
    }
    #endregion Qubit Subcircuit Helpers

    private void _setActiveQubit(Qubit qubit, bool isActive) {
        qubit.gameObject.SetActive(isActive);
    }
}
