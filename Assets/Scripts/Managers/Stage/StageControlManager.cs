using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Nito.Collections;

using StateMachines.CSM;
using StateMachines.QSM;

/*
Known issues when switching...
    - ui is not changing - look into this, stageuimanager, and qqvscript
*/

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
    public sealed class StageControlManager : Manager<StageControlManager>
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
                
                // enable and disable scripts 
                if (_currControllable != null) _currControllable.enabled = false;
                if (value != null) value.enabled = true;

                _currControllable = value;
            }
        }
        public Rigidbody2D CurrentRB => CurrentControllable?.GetComponent<Rigidbody2D>();
        public BoxCollider2D CurrentBox => CurrentControllable?.GetComponent<BoxCollider2D>();
        private Deque<Controllable> _controllables = new Deque<Controllable>(); 
        private Player _player;

        // camera for following current controllable
        private Camera _mainCamera; 
        private const float _smoothCameraSpeed = 0.125f;
        [SerializeField] private Vector3 _cameraOffset;

        // layers
        [SerializeField] private LayerMask _plaformLayerMask;
        public LayerMask PlatformLayerMask => _plaformLayerMask;
        [SerializeField] private LayerMask _defaultLayerMask;
        public LayerMask DefaultLayerMask => _defaultLayerMask;

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
            CurrentControllable = _player = SpawnManager.Instance.SpawnPlayer();
            _controllables.AddToBack(CurrentControllable);

            // set up main camera
            _mainCamera = Camera.main;

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
            // await currQSM.InitializeState(currQSMState); // <--
        }

        private async UniTaskVoid Update() {
            if (IsToggleQVVTriggered()) StageUIManager.Instance.ToggleQQVPanel();
            if (IsSwitchTriggered() && _controllables.Count > 1 && IsControllableStanding()) SwitchControllable();
            
            await _csm.CurrentState.HandleInput();
            await _csm.CurrentState.LogicUpdate();
        }

        private async UniTaskVoid FixedUpdate() {
            await _csm.CurrentState.PhysicsUpdate();

            // camera update
            if (_currControllable != null) {
                Transform targetTransform = _currControllable.transform;
                Transform camTransform = _mainCamera.transform;

                Vector3 destPos = targetTransform.position + _cameraOffset;
                Vector3 smoothPos = Vector3.Lerp(camTransform.position, destPos, _smoothCameraSpeed);

                camTransform.position = smoothPos;
            }
        }
        #endregion Event Methods

        #region Input Getters
        public float SidewaysInputValue() {
            return _stageInputs?.Controllable.Movement.ReadValue<float>() ?? 0;
        }
        public bool IsJumpTriggered() {
            return _stageInputs?.Controllable.Jump.triggered ?? false;
        }
        public bool IsSplitToggled() {
            return _stageInputs?.Controllable.Split.triggered ?? false;
        }
        public bool IsSwitchTriggered() {
            return _stageInputs?.Controllable.Switch.triggered ?? false;
        }
        public bool IsSpawnWaveTriggered() {
            return _stageInputs?.Controllable.SpawnWave.triggered ?? false;
        }
        public bool IsToggleQVVTriggered() {
            return _stageInputs?.StageUI.ToggleQQV.triggered ?? false;
        }
        
        public bool MovementOccured() {
            return SidewaysInputValue() != 0 || IsJumpTriggered();
        }

        public bool IsControllableStanding() {
            return _csm.CurrentState == StandingState;
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

        #region Controllable Methods
        public void AddControllableToBack(Controllable controllable) {
            _controllables.AddToBack(controllable);
        }

        /// <summary>
        /// Rotate left when switching between different controllables.
        /// </summary>
        public void SwitchControllable() {
            if (_currControllable.IsBusy) _currControllable.CancelForNotBeingNearGate(); 

            // move old to the back
            _controllables.RemoveFromFront();
            _controllables.AddToBack(_currControllable);

            // set curr to new 
            CurrentControllable = _controllables[0];
        }
        #endregion Controllable Methods
    }
}