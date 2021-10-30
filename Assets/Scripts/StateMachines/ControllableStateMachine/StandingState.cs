using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

using Managers;

namespace StateMachines.CSM {
    /// <summary> 
    /// The controllable is in a standing state.
    /// </summary>
    public sealed class StandingState : CSMState
    { 
        private bool _isJump;
        
        public StandingState(StageControlManager controlManager, SoundManager soundManager, CSM stateMachine) : base(controlManager, soundManager, stateMachine) {}
        
        public override async UniTask Enter() {
            await base.Enter();
            _isJump = false;
        } 
        public override async UniTask HandleInput() {
            await base.HandleInput();
            _isJump = _ctrlManager.IsJumpTriggered();
        } 
        public override async UniTask LogicUpdate() {
            await base.LogicUpdate();
            if (_isGrounded && _isJump) {
                await _stateMachine.ChangeState(_ctrlManager.JumpingState);
            }
            else if (!_isGrounded) {
                float onGroundY = _checkIfAboveGround();
                Transform transform = _ctrlManager.CurrentControllable?.transform;

                if (onGroundY != 0f && transform != null) transform.position = new Vector2(transform.position.x, onGroundY);
                else await _ctrlManager.DestroyCurrentControllable();
            }
        } 
        public override async UniTask PhysicsUpdate() {
            await base.PhysicsUpdate();
        } 
        public override async UniTask Exit() {
            await base.Exit();
        } 
    }
}
