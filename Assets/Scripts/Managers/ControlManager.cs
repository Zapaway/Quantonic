using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

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
        // TODO 
        // track player and clones so that you can pass control between them using stageinput
        // create controllablestate
        private StageInputs _stageInputs;  

        protected override void Awake()
        {
            base.Awake();
            GetStageInputs();
        }

        private async UniTaskVoid OnEnable() {
            // reference is null when GameManager is instantiated, so wait
            await UniTask.WaitUntil(() => _stageInputs != null);
            _stageInputs.Enable();
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