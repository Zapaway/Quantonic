using System.Collections.Generic;
using UnityEngine;

using Quantum;
using Quantum.Operators;
using Cysharp.Threading.Tasks;

/// <summary> 
/// GameObject that applies a binary quantum operator onto a Qubit.
/// </summary>
public sealed class BinaryGate : Gate<BinaryOperator>
{
    #region Fields/Properties
    private enum BinaryGateType : ushort {
        CNOT,
        SWAP
    }
    [SerializeField] private BinaryGateType _binaryType;

    private const int _capacity = 2;
    public override int Capacity => _capacity;
    #endregion Fields/Properties

    private void Awake() {
        // determine what unary operator to use 
        switch (_binaryType) {
            case BinaryGateType.CNOT:
                _operator = QuantumFactory.cnotOperator;
                break;
            case BinaryGateType.SWAP:
                _operator = QuantumFactory.swapOperator;
                break;
        }
    }

    protected override async UniTaskVoid GateCollisionAction(Collision2D collision)
    {
        await UniTask.Yield();
        throw new System.NotImplementedException();
    }

    protected override void _apply(Controllable controllable, List<int[]> indices)
    {
        throw new System.NotImplementedException();
    }
}
