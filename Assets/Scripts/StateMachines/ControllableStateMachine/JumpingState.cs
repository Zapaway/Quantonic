using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

using Managers;

/*
TODO: 
    Jump()
    - fix issue where controllable can teleport out of stage when teleporting up/jumping
    - figure out a way to kill the controllable if they are not above ground
*/

namespace StateMachines.CSM {
    /// <summary> 
    /// The controllable is in a jumping state.
    /// </summary>
    public sealed class JumpingState : CSMState
    { 
        private const float _jumpHeight = 5;  // how far does the controllable teleport up (DO NOT SET THIS TO ZERO)
        private const float _jumpDuration = 5;
        private bool _isFinishedJump;  // use this instead of _isGrounded as it is more reliable (accurate)

        private CancellationTokenSource _cancelJumpSource;

        public JumpingState(StageControlManager controlManager, CSM stateMachine) : base(controlManager, stateMachine) {}

        public override async UniTask Enter() {
            await base.Enter();
            
            _isFinishedJump = false;
            _cancelJumpSource = new CancellationTokenSource();
            Jump().Forget();
        } 
        public override async UniTask HandleInput() {
            await base.HandleInput();
        } 
        public override async UniTask LogicUpdate() {
            await base.LogicUpdate();

            if (_isFinishedJump) {
                await _stateMachine.ChangeState(_ctrlManager.StandingState);
            } else {
                // TODO Add death here.
            }
        } 
        public override async UniTask PhysicsUpdate() {
            await base.PhysicsUpdate();
        } 
        public override async UniTask Exit() {
            await base.Exit();

            // just in case if it is a forced exit, then the controllable has to stop jumping/being in the air
            if (!_isFinishedJump) _cancelJumpSource.Cancel();

            _cancelJumpSource.Dispose();
        } 

        private async UniTaskVoid Jump() {
            try {
                Transform transform = _ctrlManager.CurrentControllable?.transform;
                Rigidbody2D rigidbody = _ctrlManager.CurrentRB;
                        
                async UniTask _jumping() {
                    transform.Translate(Vector3.up * _jumpHeight);
                    await UniTask.Delay(TimeSpan.FromSeconds(_jumpDuration), ignoreTimeScale: false);

                    float onGroundY = _checkIfAboveGround();
                    if (onGroundY != 0f) {
                        transform.position = new Vector2(transform.position.x, onGroundY);
                    } 
                    _isFinishedJump = true;
                }
                bool isCanceled = await _jumping().AttachExternalCancellation(_cancelJumpSource.Token).SuppressCancellationThrow();
            }
            catch (Exception e) { Debug.Log(e); };
        }
    }
}
