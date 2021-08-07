using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace StateMachines.CSM {
    /// <summary>
    /// Handles movement state of controllables. 
    /// There should only be one of these.
    /// All states can be disabled via disabling specific stage input.
    /// </summary>
    public sealed class CSM : StateMachine<CSMState>
    {
        private CSMState _currState;
        public override CSMState CurrentState {get => _currState;}

        public override async UniTask InitializeState(CSMState initState)
        {
            _currState = initState;
            await _currState.Enter();
        }
        public override async UniTask ChangeState(CSMState newState)
        {
            await _currState.Exit();

            _currState = newState;
            await _currState.Enter();
        }
    }
}
