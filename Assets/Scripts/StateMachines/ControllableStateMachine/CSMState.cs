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
        
        public CSMState(ControlManager controlManager, CSM stateMachine)
        {
            _ctrlManager = controlManager;
            _stateMachine = stateMachine;
        }

        public virtual async UniTask Enter() {
        } 
        public virtual async UniTask HandleInput() {
        } 
        public virtual async UniTask LogicUpdate() {
        } 
        public virtual async UniTask PhysicsUpdate() {
        } 
        public virtual async UniTask Exit() {
        } 
    }
}
