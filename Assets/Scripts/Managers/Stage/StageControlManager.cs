using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using Nito.Collections;
using testing = System.Diagnostics;

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
    /// <description>Keeps track of composite quantum state</description>
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
        public Rigidbody2D CurrentRB => CurrentControllable?.Rigidbody2D;
        public BoxCollider2D CurrentBox => CurrentControllable?.BoxCollider2D;
        private Deque<Controllable> _controllables = new Deque<Controllable>(); 
        private Player _player;
        public int PlayerGameObjID => _player.gameObject.GetInstanceID();

        // camera for following current controllable
        private Camera _mainCamera; 
        private const float _smoothCameraSpeed = 0.125f;
        private Vector3 _originalCameraOffset;
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

        // keeps track of stage's state
        private bool _isStageFinished = false;
        public bool IsStageFinished => _isStageFinished;
        #endregion Fields/Properties

        // special mode
        [SerializeField] private bool _startWithBareMinimum;

        #region Event Methods
        protected override void Awake()
        {
            base.Awake();

            _stageInputs = new StageInputs();
            // if (_startWithBareMinimum) SetOnlyMovementActive();

            // set up qubit circuit to listen to the current controllable changed event
            circ.InitQubitCircuit(this);

            // player should always be the default current controllable
            CurrentControllable = _player = SpawnManager.Instance.SpawnPlayer(TimeOutAction);
            _controllables.AddToBack(CurrentControllable);

            _mainCamera = Camera.main;
            _originalCameraOffset = _cameraOffset;

            _jumpingState = new JumpingState(this, SoundManager.Instance, _csm);
            _standingState = new StandingState(this, SoundManager.Instance, _csm);
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
            if (IsToggleQVVTriggered()) StageUIManager.Instance.ToggleQubitPanels();
            if (IsSwitchTriggered() && _controllables.Count > 1 && IsControllableStanding()) SwitchControllable();
            
            if (_currControllable != null) {
                await _csm.CurrentState.HandleInput();
                await _csm.CurrentState.LogicUpdate();
            }
        }

        private async UniTaskVoid FixedUpdate() {
            if (_currControllable != null) {
                await _csm.CurrentState.PhysicsUpdate();

                // camera update
                Transform targetTransform = _currControllable?.transform;
                if (targetTransform == null) return;

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
        /// actions
        public void SetMovementInputActive(bool isActive) {
            if (isActive) _stageInputs.Controllable.Movement.Enable();
            else _stageInputs.Controllable.Movement.Disable();
        }
        public void SetJumpInputActive(bool isActive) {
            if (isActive) _stageInputs.Controllable.Jump.Enable();
            else _stageInputs.Controllable.Jump.Disable();
        }
        public void SetSplitInputActive(bool isActive) {
            if (isActive) _stageInputs.Controllable.Split.Enable();
            else _stageInputs.Controllable.Split.Disable();
        }
        public void SetSwitchInputActive(bool isActive) {
            if (isActive) _stageInputs.Controllable.Switch.Enable();
            else _stageInputs.Controllable.Switch.Disable();
        }
        public void SetSpawnWaveInputActive(bool isActive) {
            if (isActive) _stageInputs.Controllable.SpawnWave.Enable();
            else _stageInputs.Controllable.SpawnWave.Disable();
        }

        public void SetToggleQQVInputActive(bool isActive) {
            if (isActive) _stageInputs.StageUI.ToggleQQV.Enable();
            else _stageInputs.StageUI.ToggleQQV.Disable();
        }

        /// action maps
        public void SetControllableInputActive(bool isActive) {            
            if (isActive) _stageInputs.Controllable.Enable();
            else _stageInputs.Controllable.Disable();
        }
        public void SetStageUIInputActive(bool isActive) {
            if (isActive) _stageInputs.Controllable.Enable();
            else _stageInputs.Controllable.Disable();
        }
        public void SetPauseMenuUIInputActive(bool isActive) {
            if (isActive) _stageInputs.PauseMenuUI.Enable();
            else _stageInputs.PauseMenuUI.Disable();
        }

        /// grouped action maps
        /// <summary>
        /// Playing controls are the inputs from Controllable and Stage UI action maps.
        /// </summary>
        public void SetPlayingControlsActive(bool isActive) {
            SetControllableInputActive(isActive);
            SetStageUIInputActive(isActive);
        }

        /// special
        /// <summary>
        /// Only "A" and "D" will be allowed.
        /// </summary>
        public void SetOnlyMovementActive() {
            SetPlayingControlsActive(false);
            SetMovementInputActive(true);
        }

        /// entire
        public void SetStageControlsActive(bool isActive) {
            if (isActive) _stageInputs.Enable();
            else _stageInputs.Disable();
        }
        #endregion Input Setters

        #region Control Modes 
        public void InQQVPanelMode(bool option) {
            StageUIManager.Instance.SetQubitPanelsActive(option);
            SetToggleQQVInputActive(!option);
        }

        public void ActiveQQVPanelMode(bool option) {
            StageUIManager.Instance.SetQubitPanelsActive(option);
            SetToggleQQVInputActive(option);
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

        /// <summary>
        /// Destroy everything when disabling the player.
        /// </summary>
        /// <param name="shouldResetLocal">
        /// If true, it will reset the local state instead of load in the previous one.
        /// </param>
        public async UniTask DisablePlayer(bool shouldResetLocal = false) {
            if (_csm.CurrentState != StandingState) await _csm.ChangeState(StandingState);

            // don't allow anyone to open up the qubit panels
            ActiveQQVPanelMode(false);
            
            Clone[] clones = _controllables.Where(x => x is Clone).OfType<Clone>().ToArray();
            CurrentControllable = null;
            _controllables.Clear();
            
            _player.Deactivate();
            foreach (var c in clones) Destroy(c.gameObject);
            
            await UniTask.Delay(TimeSpan.FromSeconds(3));

            StageUIManager.Instance.ResetTimer();
            CurrentControllable = _player = SpawnManager.Instance.RespawnPlayer(_player, TimeOutAction, shouldResetLocal);
            _controllables.AddToBack(CurrentControllable);

            InQQVPanelMode(false);
        }

        /// <summary>
        /// When the timer goes out, use this action.
        /// </summary>
        public async UniTask TimeOutAction() {
            SpawnManager.Instance.ResetCheckpoint();
            await DisablePlayer(shouldResetLocal: true);
        }

        /// <summary>
        /// Destroy the current controllable. 
        /// If it is a clone, it will destroy it and switch to a different clone.
        /// If it is a player, then it will disable the player.
        /// </summary>
        public async UniTask DestroyCurrentControllable() {            
            if (_currControllable == _player) {
                StageUIManager.Instance.PauseTimer();
                await DisablePlayer();
            } 
            else if (_currControllable is Clone) {
                if (_csm.CurrentState != StandingState) await _csm.ChangeState(StandingState);
                ActiveQQVPanelMode(false);

                SwitchControllable();
                Destroy(_controllables.RemoveFromBack().gameObject);

                InQQVPanelMode(false);
            }
        }

        /// <summary>
        /// Destroys a specified controllable. 
        /// If it is the current controllable, switch over to using DestroyCurrentControllable method.
        /// If it is the player, switch over to using DisablePlayer.
        /// </summary>
        public async UniTask DestroyControllable(Controllable controllable) {
            if (controllable == _currControllable) await DestroyCurrentControllable();
            else if (controllable == _player) {
                StageUIManager.Instance.PauseTimer();
                await DisablePlayer();
            }
            else if (controllable is Clone) {  // conditional statement used for safe guard against nulls
                _controllables.Remove(controllable);
                Destroy(controllable.gameObject);
            }
        }
        #endregion Controllable Methods

        #region Camera Methods

        #endregion Camera Methods
    }
}