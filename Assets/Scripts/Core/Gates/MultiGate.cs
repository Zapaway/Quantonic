using System.Collections.Generic;
using UnityEngine;

using Quantum;
using Quantum.Operators;
using Cysharp.Threading.Tasks;

/// <summary> 
/// GameObject that applies a multi quantum operator onto qubits (operators using more than two qubits).
/// </summary>
public class MultiGate : Gate<MultiOperator>
{
    #region Fields/Properties
    private enum MultiGateType : ushort { }
    [SerializeField] private MultiGateType _binaryType;
    
    public override int Capacity => -1;
    #endregion Fields/Properties

    private void Awake() {
        // determine what multi operator to use 
    }

    protected override async UniTaskVoid GateCollisionAction(Collision2D collision)
    {
        await UniTask.Yield();
        throw new System.NotImplementedException();
    }

    protected override void _apply(Controllable controllable, int[] qsIndices)
    {
        throw new System.NotImplementedException();
    }
}
