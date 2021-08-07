using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

using Managers;

namespace StateMachines.CSM {
    /// <summary> 
    /// The controllable is in a dead state.
    /// </summary>
    public sealed class StandingState : CSMState
    { 
        public StandingState(ControlManager controlManager, CSM stateMachine) : base(controlManager, stateMachine) {}
        
        public override async UniTask Enter() {
        } 
        public override async UniTask HandleInput() {
        } 
        public override async UniTask LogicUpdate() {
        } 
        public override async UniTask PhysicsUpdate() {
        } 
        public override async UniTask Exit() {
        } 
    }
}
