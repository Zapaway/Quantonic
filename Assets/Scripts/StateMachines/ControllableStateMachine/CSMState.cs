using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

using Managers;

namespace StateMachines.CSM {
    public abstract class CSMState : IState
    {
        protected StageControlManager _ctrlManager;
        protected SoundManager _soundManager;
        protected CSM _stateMachine;
        protected float _sidewaysInput;
        protected float _moveSpeed;
        protected bool _isGrounded;
        
        public CSMState(StageControlManager controlManager, SoundManager soundManager, CSM stateMachine)
        {
            _ctrlManager = controlManager;
            _soundManager = soundManager; 
            _stateMachine = stateMachine;
        }

        public virtual async UniTask Enter() {
            await UniTask.Yield();

            _sidewaysInput = 0;
            _moveSpeed = 5;
        } 
        public virtual async UniTask HandleInput() {
            await UniTask.Yield();

            _isGrounded = _checkIfGrounded();
            _sidewaysInput = _ctrlManager.SidewaysInputValue();
        } 
        public virtual async UniTask LogicUpdate() {
            await UniTask.Yield();
        } 
        public virtual async UniTask PhysicsUpdate() {
            await UniTask.Yield();

            Rigidbody2D rigidbody = _ctrlManager.CurrentRB;
            if (rigidbody != null) rigidbody.velocity = new Vector2(_moveSpeed * _sidewaysInput, rigidbody.velocity.y);
        } 
        public virtual async UniTask Exit() {
            await UniTask.Yield();
        } 

        private bool _checkIfGrounded() {
            BoxCollider2D col = _ctrlManager.CurrentBox;
            
            if (col != null) {
                Collider2D overlap = Physics2D.OverlapBox(
                    col.bounds.center, col.bounds.size, 
                    0f, _ctrlManager.PlatformLayerMask
                );

                return overlap != null ? true : false;
            } 
            return false;
        }

        protected float _checkIfAboveGround() {
            BoxCollider2D col = _ctrlManager.CurrentBox;
            if (col == null) return 0f;
            
            RaycastHit2D hit = Physics2D.BoxCast(
                col.bounds.center, col.bounds.size, 
                0f, Vector2.down, Mathf.Infinity, 
                _ctrlManager.PlatformLayerMask
            );

            // adjust for controllable to "teleport on"
            return hit.collider == null 
                ? 0f 
                : (
                    (hit.transform.position.y + _ctrlManager.CurrentControllable.transform.localScale.y) 
                    + 
                    (hit.transform.position.y + hit.transform.localScale.y)
                )/2; 
        }
    }
}
