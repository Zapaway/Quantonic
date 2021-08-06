using System.Collections.ObjectModel;
using UnityEngine;

using Quantum;
using Quantum.Operators;

/// <summary> 
/// GameObject that applies a unary quantum operator onto a Qubit.
/// </summary>
public sealed class UnaryGate : Gate<UnaryOperator>
{
    #region Fields/Properties
    // choices for a unary operator in the Unity inspector
    private enum UnaryGateType : ushort {
        PauliX,
        PauliY,
        PauliZ,
        Hadamard,
    }
    [SerializeField] private UnaryGateType _unaryType;
    
    private const int _capacity = 1;
    public override int Capacity => _capacity;
    #endregion Fields/Properties

    private void Awake() {
        // determine what unary operator to use 
        switch (_unaryType) {
            case UnaryGateType.PauliX:
                _operator = QuantumFactory.pauliXOperator;
                break;
            case UnaryGateType.PauliY:
                _operator = QuantumFactory.pauliYOperator;
                break;
            case UnaryGateType.PauliZ:
                _operator = QuantumFactory.pauliZOperator;
                break;
            case UnaryGateType.Hadamard:
                _operator = QuantumFactory.hadamardOperator;
                break;
        }
    }

    protected override void _apply(ReadOnlyCollection<QuantumState> quantumStates)
    {
        throw new System.NotImplementedException();
    }

    // TODO: Override OnTriggerEnter
}
