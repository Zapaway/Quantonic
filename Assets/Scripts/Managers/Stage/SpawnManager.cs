using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

using Quantum;

namespace Managers {
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// <description>Handles all spawning and despawning in a stage, except for when the player starts (handled in ControlManager)</description>
    /// </item>
    /// <item>
    /// <description>Contains prefabs for instantiation</description>
    /// </item>
    /// <item>
    /// <description>Keeps track of checkpoints (teleports to the checkpoint)</description>
    /// </item>
    /// <item>
    /// <description>Saves information about player when checkpoints are triggered</description>
    /// </item>
    /// </list></summary>
    public sealed class SpawnManager : Manager<SpawnManager>
    {
        /// prefabs
        [SerializeField] private GameObject _qubitPrefab;  // default is the ground state
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameObject _clonePrefab;
        [SerializeField] private GameObject _playerWavePrefab;
        
        /// checkpoints
        private Vector3 _spawnPoint;
        private Vector3 _lastestCheckpoint;  // first should always be spawn point that is the location of the spawn manager
        private CheckpointScript[] _checkpoints;

        /// scene data
        private GameObject[] _respawnableEnemies;  // all enemies that can be respawned will be tagged as "Enemy|Respawn"
        private GameObject[] _respawnableQubitCollectables;  // all qubit collectables will be tagged as "Qubit|Respawn"
        
        // used for perserving player state so that respawning will revert to the appropiate previous states
        private sealed class LocalSaveStageState {
            public List<GameObject> respawnableEnemiesUnactivated = new List<GameObject>();
            public List<GameObject> respawnableQubitCollectablesUnactivated = new List<GameObject>();
            public QuantumState[] playerQuantumStates;
            public (int controlQSIndex, int targetQSIndex)[] playerEntanglementInfo;
        }
        private LocalSaveStageState _latestSaveState = new LocalSaveStageState();
        
        private bool _isPlayerSpawned = false; 

        protected override void Awake()
        {
            base.Awake();

            // gather all respawnable objs
            _respawnableEnemies = GameObject.FindGameObjectsWithTag("Enemy|Respawn");
            _respawnableQubitCollectables = GameObject.FindGameObjectsWithTag("Qubit|Respawn");

            // gather checkpoint info 
            _spawnPoint = transform.position;
            _checkpoints = (
                from checkpointGo 
                in GameObject.FindGameObjectsWithTag("Checkpoint") 
                select checkpointGo.GetComponent<CheckpointScript>()
            ).ToArray();
            ResetCheckpoint();
        }

        #region Instantiation Methods
        /// <summary>
        /// Spawn a qubit.
        /// </summary>
        public Qubit MakeQubit(float xOffset) {
            GameObject qubit = Instantiate(
                _qubitPrefab, 
                Vector3.zero + new Vector3(xOffset, 0, 0), 
                _qubitPrefab.transform.rotation
            );

            return qubit.GetComponent<Qubit>();
        }
        public Qubit MakeQubit(Vector3 position) {
            GameObject qubit = Instantiate(
                _qubitPrefab,
                position,
                _qubitPrefab.transform.rotation
            );

            return qubit.GetComponent<Qubit>();
        }

        /// <summary>
        /// Create a row of qubits that are left offsetted from the origin (0, 0). All qubits will be disabled initally. 
        /// </summary>
        /// <returns> A list of qubits, with the leftmost qubit corresponding to the 0 index. </returns>
        public List<Qubit> MakeQubits(int n, int spacing, uint leftOffset) {
            List<Qubit> res = new List<Qubit>();

            for (int i = n - 1; i >= 0; --i) {
                Qubit qubit = MakeQubit(-leftOffset - i * spacing);
                qubit.gameObject.SetActive(false);
                res.Add(qubit);
            }

            return res;
        }

        /// <summary>
        /// Spawn a player and start the timer. 
        /// </summary>
        public Player SpawnPlayer(Func<UniTask> timeRunOutActionAsync) {
            if (!_isPlayerSpawned) {
                
                GameObject playerObj = Instantiate(
                    _playerPrefab,
                    _lastestCheckpoint,
                    _playerPrefab.transform.rotation
                );
                
                _completePlayerSetup(timeRunOutActionAsync);
                return playerObj.GetComponent<Player>();
            } 

            return null;
        }
        /// <summary>
        /// Respawn the player (by creating or enabling) and start the timer.
        /// </summary>
        /// <param name="shouldResetLocal">
        /// If true, it will reset the local state instead of load in the previous one.
        /// </param>
        public Player RespawnPlayer(Player player, Func<UniTask> timeRunOutActionAsync, bool shouldResetLocal = false) {
            _isPlayerSpawned = player != null;

            if (_isPlayerSpawned) {
                player.gameObject.transform.position = _lastestCheckpoint;

                if (shouldResetLocal) _resetLocalState(player);
                else LoadLocalState(player).Forget();

                _completePlayerSetup(timeRunOutActionAsync);
            }
            else player = SpawnPlayer(timeRunOutActionAsync);

            return player;
        }
        private void _completePlayerSetup(Func<UniTask> timeRunOutActionAsync) {
            StageUIManager.Instance.TimerRunOutActionAsync = timeRunOutActionAsync;
            StageUIManager.Instance.StartTimer();
                
            _isPlayerSpawned = true;
        }

        /// <summary>
        /// Spawn a clone around a specific controllable on the left side.
        /// </summary>
        public Clone SpawnClone(Controllable controllable, float spacing) {
            Transform ctrlableTrans = controllable.transform;
            Vector2 leftSideOfCtrlable = new Vector2(ctrlableTrans.position.x - ctrlableTrans.localScale.x/2 - spacing, ctrlableTrans.position.y);
            GameObject cloneObj = Instantiate(
                _clonePrefab,
                leftSideOfCtrlable,
                _clonePrefab.transform.rotation
            );

            return cloneObj.GetComponent<Clone>();
        }

        /// <summary>
        /// Spawn a player wave ability from the given coords.
        /// </summary>
        public async UniTask SpawnPlayerWave(Vector2 originPosition) {
            float aimAng = 0;

            // if the cursor is held down, then launch the wave towards the cursor
            if (GlobalControlManager.Instance.IsCursorHeldDown) {  
                aimAng = GlobalControlManager.Instance.GetAngleRelativeToMouse(originPosition);
            }

            GameObject waveObj = Instantiate(
                _playerWavePrefab,
                originPosition,
                Quaternion.Euler(0, 0, aimAng)
            );
            SoundManager.Instance.StageSounds.PlayWaveShootSFX();
            await UniTask.WaitUntil(() => waveObj == null);  // wait until wave is destroyed
        }
        
        
        #endregion Instantiation Methods

        #region Checkpoint and Save Point Methods
        /// <summary>
        /// Set the checkpoint to the touched checkpoint and teleport there.
        /// </summary>
        /// <param name="player">
        /// If player is not null, then it will save the local state.
        /// </param>
        public void SetCheckpoint(CheckpointScript checkpointScript, Player player = null) {
            Transform checkTransform = checkpointScript.gameObject.transform;
            transform.position = _lastestCheckpoint = (
                checkTransform.position - Vector3.up * checkTransform.localScale.y * 2
            ); 

            if (player != null) SaveLocalState(player);
        }
        /// </summary>
        /// Set the checkpoint to the spawnpoint, teleport the manager there, and reset all states of the checkpoints.
        /// <summary>
        public void ResetCheckpoint() {
            transform.position = _lastestCheckpoint = _spawnPoint;
            foreach (var chkpScript in _checkpoints) chkpScript.ResetCheckpointState();
        }

        /// <summary>
        /// Add onto the unactived respawnable enemy list.
        /// </summary>
        public SpawnManager AddUnactivatedRespawnableEnemy(GameObject enemyObj) {
            _latestSaveState.respawnableEnemiesUnactivated.Add(enemyObj);
            return this;
        }
        /// <summary>
        /// Add onto the unactivated qubit collectable list.
        /// </summary>
        public SpawnManager AddUnactivatedQubitCollectable(GameObject qubitCollectableObj) {
            _latestSaveState.respawnableQubitCollectablesUnactivated.Add(qubitCollectableObj);
            return this;
        }

        /// <summary>
        /// Save player quantum state and clear the unenabled respawnable enemy list from here.
        /// </summary>
        public SpawnManager SaveLocalState(Player player) {
            _latestSaveState.playerQuantumStates = (from sv in player.SubcircVectors select new QuantumState(sv[0], sv[1])).ToArray();
            _latestSaveState.playerEntanglementInfo = player.SubcircEntanglementInfo;
            
            _clearRespawnablesInLocal();

            return this;
        }
        /// <summary>
        /// Activate the player with their last saved player quantum state. Also activate every respawnable.
        /// </summary>
        public async UniTask<SpawnManager> LoadLocalState(Player player) {
            player.Activate(_latestSaveState.playerQuantumStates);
            player.RecalculateQubitSubcircit();

            int entangleCount = _latestSaveState.playerEntanglementInfo.Count();
            List<int> entangledControlQSIndices = new List<int>(entangleCount/2);
            for (int i = 0; i < entangleCount; ++i) {
                var (controlQSIndex, targetQSIndex) = _latestSaveState.playerEntanglementInfo[i];
                if (controlQSIndex == -1 || entangledControlQSIndices.Contains(controlQSIndex)) continue;

                await player.ApplyForcedEntanglement(controlQSIndex, targetQSIndex);
                entangledControlQSIndices.Add(controlQSIndex);
            }

            _setGameObjectsActive(_latestSaveState.respawnableEnemiesUnactivated);
            _setGameObjectsActive(_latestSaveState.respawnableQubitCollectablesUnactivated);
            _clearRespawnablesInLocal();

            return this;
        }
        /// <summary>
        /// Reset the local state and enable every game object that is respawnable. Activate the player too with initial qubits.
        /// </summary>
        private SpawnManager _resetLocalState(Player player) {
            player.Activate();
            SaveLocalState(player);

            _setGameObjectsActive(_respawnableEnemies);
            _setGameObjectsActive(_respawnableQubitCollectables);

            return this;
        }   

        private void _setGameObjectsActive(IEnumerable<GameObject> gameObjects) {
            foreach (var go in gameObjects) go.SetActive(true);
        }
        private void _clearRespawnablesInLocal() {
            _latestSaveState.respawnableEnemiesUnactivated.Clear();
            _latestSaveState.respawnableQubitCollectablesUnactivated.Clear();
        }
        #endregion Checkpoint and Save Point Methods
    }
}

