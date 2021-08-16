using UnityEngine;
using Cysharp.Threading.Tasks;

using Managers;
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

    /// <summary>
    /// Apply the operator on the qubits selected.
    /// </summary>
    protected async override UniTaskVoid OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Controllable")) {
            Controllable controllable = ControlManager.Instance.CurrentControllable;
            // TODO: ask the current controllable what qubit to use (for now, use the first qubit in the player)
            int[] qubitIndex = new int[_capacity]{ controllable.AskForSingleQubitIndex() };

            base.OnCollisionEnter2D(collision).Forget();

            _apply(controllable, qubitIndex);

            await UniTask.Yield();
        }
    }

    protected override void _apply(Controllable controllable, int[] indices)
    {
        controllable.ApplyUnaryOperator(_operator, indices[0]);
    }
}
