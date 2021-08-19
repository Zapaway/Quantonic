using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Nito.Collections;

using StateMachines.CSM;

namespace Managers {
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
    public sealed class ControlManager : Manager<ControlManager>
    {
        #region Fields/Properties
        // player & clones (controllables)
        public Player Player {get; private set;} 
        public Rigidbody2D PlayerRB {get; private set;}
        public BoxCollider2D PlayerBox {get; private set;}
        
        public Controllable CurrentControllable {get; private set;}

        private Deque<Controllable> _controllables = new Deque<Controllable>(); 

        // platform layer
        [SerializeField] private LayerMask _plaformLayerMask;
        public LayerMask PlatformLayerMask => _plaformLayerMask;

        // inputs that the control manager handle
        private StageInputs _stageInputs; 

        // the control state machine it uses to affect the controllables
        private CSM _csm = new CSM();
        private JumpingState _jumpingState;
        public JumpingState JumpingState => _jumpingState;
        private StandingState _standingState;
        public StandingState StandingState => _standingState;
        #endregion Fields/Properties

        #region Event Methods
        protected override void Awake()
        {
            base.Awake();

            // create stage inputs
            _stageInputs = new StageInputs();

            // cache & configure player components
            Player player = SpawnManager.Instance.SpawnPlayer();  // this is the starting point of the stage
            GameObject playerGameObj = player.gameObject;

            Player = player;
            PlayerRB = playerGameObj.GetComponent<Rigidbody2D>();
            PlayerRB.constraints = RigidbodyConstraints2D.FreezeRotation;
            PlayerBox = playerGameObj.GetComponent<BoxCollider2D>();

            // player should always be the default current controllable
            CurrentControllable = player;
            _controllables.AddToBack(player);

            // initalize the controllable states
            _jumpingState = new JumpingState(this, _csm);
            _standingState = new StandingState(this, _csm);
        }

        private void OnEnable() {
            _stageInputs.Controllable.Enable();
            _stageInputs.StageUI.Enable();
        }

        private void OnDisable() {
            _stageInputs?.Disable();
        }

        private async UniTaskVoid Start() {
            await _csm.InitializeState(_standingState);
        }

        private async UniTaskVoid Update() {
            if (ToggleQVVTriggered()) {
                StageUIManager.Instance.ToggleQQVPanel();
            }
            
            await _csm.CurrentState.HandleInput();
            await _csm.CurrentState.LogicUpdate();
        }

        private async UniTaskVoid FixedUpdate() {
            await _csm.CurrentState.PhysicsUpdate();
        }
        #endregion Event Methods

        #region Input Getters
        public float SidewaysInputValue() {
            return _stageInputs?.Controllable.Movement.ReadValue<float>() ?? 0;
        }
        public bool JumpTriggered() {
            return _stageInputs?.Controllable.Jump.triggered ?? false;
        }
        public bool ToggleQVVTriggered() {
            return _stageInputs?.StageUI.ToggleQQV.triggered ?? false;
        }
        
        public bool MovementOccured() {
            return SidewaysInputValue() != 0 || JumpTriggered();
        }
        #endregion Input Getters

        #region Input Setters 
        public void SetControllableInputActive(bool isActive) {
            if (isActive) _stageInputs.Controllable.Enable();
            else _stageInputs.Controllable.Disable();
        }
        public void SetStageUIInputActive(bool isActive) {
            if (isActive) _stageInputs.Controllable.Enable();
            else _stageInputs.Controllable.Disable();
        }
        public void SetToggleQQVInputActive(bool isActive) {
            if (isActive) _stageInputs.StageUI.ToggleQQV.Enable();
            else _stageInputs.StageUI.ToggleQQV.Disable();
        }
        public void SetJumpInputActive(bool isActive) {
            if (isActive) _stageInputs.Controllable.Jump.Enable();
            else _stageInputs.Controllable.Jump.Disable();
        }

        /// <summary>
        /// Playing controls are the inputs from Controllable and Stage UI action maps.
        /// </summary>
        public void SetPlayingControlsActive(bool isActive) {
            SetControllableInputActive(isActive);
            SetStageUIInputActive(isActive);
        }
        #endregion Input Setters

        #region Control Modes
        public void InQQVPanelMode(bool option) {
            StageUIManager.Instance.SetQQVPanelActive(option);
            SetToggleQQVInputActive(!option);
        }
        #endregion Control Modes
    }
}