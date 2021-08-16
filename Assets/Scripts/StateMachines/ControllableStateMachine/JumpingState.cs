using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

using Managers;

/*
TODO: 
    Jump()
    - fix issue where player can teleport out of stage when teleporting up/jumping
    - figure out a way to kill the player if they are not above ground

    - go into CSMState and put _isGrounded there instead of here (also bring the function over)
*/

namespace StateMachines.CSM {
    /// <summary> 
    /// The controllable is in a jumping state.
    /// </summary>
    public sealed class JumpingState : CSMState
    { 
        private float _jumpHeight = 5;  // how far does the player teleport up (DO NOT SET THIS TO ZERO)
        private float _jumpDuration = 5;

        public JumpingState(ControlManager controlManager, CSM stateMachine) : base(controlManager, stateMachine) {}

        public override async UniTask Enter() {
            await base.Enter();
            Jump().Forget();
        } 
        public override async UniTask HandleInput() {
            await base.HandleInput();
        } 
        public override async UniTask LogicUpdate() {
            await base.LogicUpdate();
            if (_isGrounded) {
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
        } 

        private async UniTaskVoid Jump() {
            Transform transform = _ctrlManager.Player.transform;
            Rigidbody2D rigidbody = _ctrlManager.PlayerRB;
            
            rigidbody.gravityScale = 0f;  // allow the object to stay up in the air 

            transform.Translate(Vector3.up * _jumpHeight);
            await UniTask.Delay(TimeSpan.FromSeconds(_jumpDuration), ignoreTimeScale: false);

            float onGroundY = CheckIfAboveGround();
            if (onGroundY != 0f) {
                transform.position = new Vector2(transform.position.x, onGroundY);
            } 
            else {
                // TODO death here or something
            }

            rigidbody.gravityScale = 1f;  
        }
        
        private float CheckIfAboveGround() {
            BoxCollider2D col = _ctrlManager.PlayerBox;
            RaycastHit2D hit = Physics2D.BoxCast(
                col.bounds.center, col.bounds.size, 
                0f, Vector2.down, Mathf.Infinity, 
                _ctrlManager.PlatformLayerMask
            );

            // adjust for player to "teleport on"
            return hit.collider == null 
                ? 0f 
                : hit.transform.position.y + _ctrlManager.Player.transform.localScale.y; 
        }
    }
}
