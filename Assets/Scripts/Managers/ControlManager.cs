using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

using StateMachines.CSM;

namespace Managers {
    #region Summary
    /// <summary>
    /// This is a stage manager. Does the following below...
    /// <list type="bullet">
    /// <item>
    /// <description>Handles input in a stage</description>
    /// </item>
    /// <item>
    /// <description>Passes control between player and their clones</description>
    /// </item>
    /// <item>
    /// <description>Keeps track of ControllableState</description>
    /// </item>
    /// <item>
    /// <description>Keeps track of composite quantum state </description>
    /// </item>
    /// </list></summary>
    #endregion Summary
    public sealed class ControlManager : Manager<ControlManager>
    {
        // inputs that the control manager handle
        private StageInputs _stageInputs; 

        // the control state machine it uses to affect the controllables
        private CSM _csm = new CSM();
        private JumpingState _jumpingState;
        private StandingState _standingState;
        private DeadState _deadState;

        protected override void Awake()
        {
            base.Awake();
            GetStageInputs();

            // initalize the controllable states
            _jumpingState = new JumpingState(this, _csm);
            _standingState = new StandingState(this, _csm);
            _deadState = new DeadState(this, _csm);
        }

        private async UniTaskVoid OnEnable() {
            // reference is null when GameManager is instantiated, so wait
            await UniTask.WaitUntil(() => _stageInputs != null);
            _stageInputs.Controllable.Enable();
        }

        private void OnDisable() {
            _stageInputs?.Disable();
        }

        /// <summary>
        /// Attempt to get the reference to GameManager's stage input.
        /// </summary>
        public void GetStageInputs() {
            _stageInputs = GameManager.Instance?.StageInputs;
        } 
    }
}