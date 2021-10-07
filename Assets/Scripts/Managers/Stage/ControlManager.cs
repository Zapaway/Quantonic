using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Nito.Collections;

using StateMachines.CSM;

namespace Managers {
    public sealed class OnCurrentControllableChangedEventArgs : EventArgs {
        public Controllable OldValue {get; set;}
        public Controllable NewValue {get; set;}
    }

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
        private StageInputs _stageInputs; 

        // qubit tracker
        public readonly QubitCircuit circ = new QubitCircuit();

        // events & delegates
        public event EventHandler<OnCurrentControllableChangedEventArgs> OnCurrentControllableChanged;


        // controllables      
        private Controllable _currControllable = null;  
        public Controllable CurrentControllable {
            get => _currControllable;
            private set {
                if (_currControllable != value) {
                        OnCurrentControllableChanged?.Invoke(this, 
                        new OnCurrentControllableChangedEventArgs(){
                            OldValue = _currControllable,
                            NewValue = value
                        }
                    );
                }

                _currControllable = value;
            }
        }
        public Rigidbody2D CurrentRB {get; private set;}
        public BoxCollider2D CurrentBox {get; private set;}

        private Deque<Controllable> _controllables = new Deque<Controllable>(); 

        // platform layer
        [SerializeField] private LayerMask _plaformLayerMask;
        public LayerMask PlatformLayerMask => _plaformLayerMask;

        // uses it to affect the controllable movement
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

            // set up qubit circuit to listen to the current controllable changed event
            circ.InitQubitCircuit(this);

            // player should always be the default current controllable
            CurrentControllable = SpawnManager.Instance.SpawnPlayer();
            CurrentRB = CurrentControllable.GetComponent<Rigidbody2D>();
            CurrentBox = CurrentControllable.GetComponent<BoxCollider2D>();
            _controllables.AddToBack(CurrentControllable);

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
            if (IsToggleQVVTriggered()) {
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
        public bool IsJumpTriggered() {
            return _stageInputs?.Controllable.Jump.triggered ?? false;
        }
        public bool IsToggleQVVTriggered() {
            return _stageInputs?.StageUI.ToggleQQV.triggered ?? false;
        }
        
        public bool MovementOccured() {
            return SidewaysInputValue() != 0 || IsJumpTriggered();
        }

        public bool IsSpawnWaveTriggered() {
            return _stageInputs?.Controllable.SpawnWave.triggered ?? false;
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