using System;
using System.Collections.Generic;
using System.Collections.Specialized;

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
    private Controllable CurrentControllable => ControlManager.Instance.CurrentControllable;

    // there should be no removing of any qubit, only setting it inactive
    private readonly List<Qubit> _allQubits;  
    public int Count => _allQubits.Count;

    // use a controllable's ID to access its subcircuit (their qubits)
    private readonly Dictionary<int, QubitSubcircuit> _allSubcircuits;

    // allows the same handler to be used across any current controllable's subcirc
    private NotifyCollectionChangedEventHandler _subcircuitCollectionChangedHandler;

    #endregion Fields/Properties

    public QubitCircuit() {
        _allQubits = new List<Qubit>(_defaultCapacity);
        _allSubcircuits = new Dictionary<int, QubitSubcircuit>();
    }
    public void InitQubitCircuit(ControlManager controlManager) {
        // set up the events
        controlManager.OnCurrentControllableChanged += (object sender, OnCurrentControllableChangedEventArgs e) => {
            Controllable oldCtrllable = e.OldValue, newCtrllable = e.NewValue;

            if (oldCtrllable != null) {
                oldCtrllable.UnsubscribeToSubcircuitCollection(_subcircuitCollectionChangedHandler);
            }
            if (newCtrllable != null) {
                newCtrllable.SubscribeToSubcircuitCollection(_subcircuitCollectionChangedHandler);
            }
        };
    }

    /// <summary>
    /// Create a qubit subcircuit. Controllable value must not be null.
    /// </summary>
    public IQubitSubcircuit CreateQubitSubcircuit(Controllable controllable) {
        if (controllable == null) {
            throw new ArgumentNullException("Controllable must be a non-null value");
        }

        QubitSubcircuit subcirc = new QubitSubcircuit(this, controllable);
        _allSubcircuits[controllable.GetHashCode()] = subcirc;

        return subcirc;
    }
    /// <summary>
    /// Get a qubit subcircuit from the dictionary. If controllable value is null, use the current
    /// controllable from the ControlManager.
    /// </summary>
    public IQubitSubcircuit GetQubitSubcircuit(Controllable controllable = null) {
        if (controllable == null) controllable = CurrentControllable;

        return _allSubcircuits[controllable.GetHashCode()];
    }

    public void AddSubcircuitHandler(NotifyCollectionChangedEventHandler handler) {
        _subcircuitCollectionChangedHandler += handler;
    }
    public void RemoveSubcircuitHandler(NotifyCollectionChangedEventHandler handler) {
        _subcircuitCollectionChangedHandler -= handler;
    }

    #region Qubit Circuit Manipulation
    /// <summary>
    /// Remove all subcircuits and set every qubit inactive.
    /// Note that there is no notification of the any subcircuits being cleared.
    /// </summary>
    public void ClearAllQubits() {
        for (int i = 0; i < _allQubits.Count; ++i) {
            _allQubits[i] = null;
        }

        _allSubcircuits.Clear();
    }
    #endregion Qubit Circuit Manipulation

    #region Qubit Subcircuit Helpers
    private int _add(Qubit newQubit) {
        _allQubits.Add(newQubit);
        return _allQubits.Count - 1;
    }

    private void _clear(Dictionary<int, int>.KeyCollection qcIndices) {
        foreach (int qcIndex in qcIndices) {
            _allQubits[qcIndex] = null;
        }
    }

    private void _removeAt(int qcIndex) {
        _allQubits[qcIndex] = null;
    }
    #endregion Qubit Subcircuit Helpers

    // private Vector<sysnum.Complex> _tensorProduct(Vector<sysnum.Complex> a, Vector<sysnum.Complex> b) {
    //     var resList = new List<sysnum.Complex[]>(a.Count);

    //     foreach (var element in a) {
    //         resList.Add(b.Multiply(element).ToArray());                
    //     }

    //     return Vector<sysnum.Complex>.Build.DenseOfEnumerable(resList.SelectMany(e => e));
    // }
}
