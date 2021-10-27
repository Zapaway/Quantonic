using System.Collections.Generic;
using UnityEngine;

using Managers;
using Quantum;
using Quantum.Operators;
using Cysharp.Threading.Tasks;

/// <summary> 
/// GameObject that applies a binary quantum operator onto a pair of qubits.
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
        // determine what binary operator to use 
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
        StageControlManager.Instance.InQQVPanelMode(true);

        List<int> res;
        if (_operator is IControlledOperator<BinaryOperator>) {
            res = await OccupiedControllable.AskForMultipleSingleQubitIndices(
                _capacity, 
                OccupiedControllable.PreventTargetedQubits,
                OccupiedControllable.SetAllowedQubitsForSubmissionBasedOnType,
                considerEntangledCase: true
            );
        }
        else {
            res = await OccupiedControllable.AskForMultipleSingleQubitIndices(_capacity);
        }
        
        if (res != null) {
            base.GateCollisionAction(collision).Forget();
            _apply(OccupiedControllable, res.ToArray());
        }

        StageControlManager.Instance.InQQVPanelMode(false);
    }

    protected override void _apply(Controllable controllable, int[] qsIndices)
    {
        controllable.ApplyBinaryOperator(_operator, qsIndices).Forget();
    }
}