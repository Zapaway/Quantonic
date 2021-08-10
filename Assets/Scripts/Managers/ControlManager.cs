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
        #region Fields/Properties
        // player & clones (controllables)
        // TODO: change _player type from GameObject to Player type
        [SerializeField] private GameObject _player;  // make it so that a spawnmanager handles spawning the player
        public GameObject Player => _player;
        public Rigidbody2D PlayerRB {get; private set;}
        public BoxCollider2D PlayerBox {get; private set;}

        [SerializeField] private GameObject _clonePrefab;
        private List<GameObject> _clones; 

        public Controllable CurrentControllable {get; private set;}

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
            GetStageInputs();

            // cache & configure player components
            PlayerRB = _player.GetComponent<Rigidbody2D>();
            PlayerRB.constraints = RigidbodyConstraints2D.FreezeRotation;
            PlayerBox = _player.GetComponent<BoxCollider2D>();

            // player should always be the default current controllable
            CurrentControllable = Player.GetComponent<Player>();

            // initalize the controllable states
            _jumpingState = new JumpingState(this, _csm);
            _standingState = new StandingState(this, _csm);
        }

        private async UniTaskVoid OnEnable() {
            // reference is null when GameManager is instantiated, so wait
            await UniTask.WaitUntil(() => _stageInputs != null);
            _stageInputs.Controllable.Enable();
        }

        private void OnDisable() {
            _stageInputs?.Disable();
        }

        private async UniTaskVoid Start() {
            await _csm.InitializeState(_standingState);
        }

        private async UniTaskVoid Update() {
            await _csm.CurrentState.HandleInput();
            await _csm.CurrentState.LogicUpdate();
        }

        private async UniTaskVoid FixedUpdate() {
            await _csm.CurrentState.PhysicsUpdate();
        }
        #endregion Event Methods

        #region Input Values Getters
        public float SidewaysInputValue() {
            return _stageInputs?.Controllable.Movement.ReadValue<float>() ?? 0;
        }
        public bool JumpTriggered() {
            return _stageInputs?.Controllable.Jump.triggered ?? false;
        }
        #endregion Input Values Getters

        /// <summary>
        /// Attempt to get the reference to GameManager's stage input.
        /// </summary>
        public void GetStageInputs() {
            _stageInputs = GameManager.Instance?.StageInputs;
        } 
    }
}