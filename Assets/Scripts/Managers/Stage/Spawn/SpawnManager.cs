using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Managers {
    /// <summary>
    /// Handles all spawning and despawning in a stage,
    /// except for when the player starts (handled in ControlManager).
    /// Also contains prefabs for instantiation.
    /// </summary>
    public sealed class SpawnManager : Manager<SpawnManager>
    {
        [SerializeField] private GameObject _qubitPrefab;  // default is the ground state
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameObject _clonePrefab;
        [SerializeField] private GameObject _playerWavePrefab;
        
        // checkpoints
        private GameObject[] _checkpoints;
        private Vector3 _lastestCheckpoint = new Vector2(0, -6.5f);
        //  - see spawnPlayer()
        
        private bool _isPlayerSpawned = false; 
        public bool IsPlayerSpawned => _isPlayerSpawned;

        protected override void Awake()
        {
            base.Awake();

            // find all checkpoints (first one should always be spawn point)
            _checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
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
                GameObject player = Instantiate(
                    _playerPrefab,
                    _lastestCheckpoint,
                    _playerPrefab.transform.rotation
                );
                
                _completePlayerSetup(timeRunOutActionAsync);
                return player.GetComponent<Player>();
            } 

            return null;
        }
        /// <summary>
        /// Respawn the player (by creating or enabling) and start the timer.
        /// </summary>
        public Player RespawnPlayer(Player player, Func<UniTask> timeRunOutActionAsync) {
            _isPlayerSpawned = player != null;

            if (_isPlayerSpawned) {
                player.gameObject.transform.position = _lastestCheckpoint;
                player.Activate();

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
            await UniTask.WaitUntil(() => waveObj == null);  // wait until wave is destroyed
        }
        
        
        #endregion Instantiation Methods

        #region Deletion Methods
        public void DestroyEveryControllable() {

        }
        public void DestroyCurrentControllable() {

        }
        #endregion Deletion Methods
    }
}

