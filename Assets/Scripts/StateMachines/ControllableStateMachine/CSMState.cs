using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

using Managers;

namespace StateMachines.CSM {
    public abstract class CSMState : IState
    {
        protected ControlManager _ctrlManager;
        protected CSM _stateMachine;
        protected float _sidewaysInput;
        protected float _moveSpeed;
        protected bool _isGrounded;
        
        public CSMState(ControlManager controlManager, CSM stateMachine)
        {
            _ctrlManager = controlManager;
            _stateMachine = stateMachine;
        }

        public virtual async UniTask Enter() {
            _sidewaysInput = 0;
            _moveSpeed = 5;

            await UniTask.Yield();
        } 
        public virtual async UniTask HandleInput() {
            _isGrounded = CheckIfGrounded();
            _sidewaysInput = _ctrlManager.SidewaysInputValue();
            
            await UniTask.Yield();
        } 
        public virtual async UniTask LogicUpdate() {
            await UniTask.Yield();
        } 
        public virtual async UniTask PhysicsUpdate() {
            Rigidbody2D rigidbody = _ctrlManager.CurrentRB;
            rigidbody.velocity = new Vector2(_moveSpeed * _sidewaysInput, rigidbody.velocity.y);
            await UniTask.Yield();
        } 
        public virtual async UniTask Exit() {
            await UniTask.Yield();
        } 

        private bool CheckIfGrounded() {
            BoxCollider2D col = _ctrlManager.CurrentBox;
            Collider2D overlap = Physics2D.OverlapBox(
                col.bounds.center, col.bounds.size, 
                0f, _ctrlManager.PlatformLayerMask
            );

            return overlap != null ? true : false;
        }
    }
}
