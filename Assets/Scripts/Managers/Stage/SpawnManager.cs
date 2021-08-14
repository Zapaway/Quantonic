using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers {
    /// <summary>
    /// Handles all spawning in a stage,
    /// except for when the player starts (handled in ControlManager).
    /// Also contains prefabs for instantiation.
    /// </summary>
    public sealed class SpawnManager : Manager<SpawnManager>
    {
        [SerializeField] private GameObject _qubitPrefab;  // default is the ground state
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameObject _clonePrefab;
        
        private bool _isPlayerSpawned = false; 
        public bool IsPlayerSpawned => _isPlayerSpawned;

        protected override void Awake()
        {
            base.Awake();
        }

        #region Instantiation Methods
        /// <summary>
        /// Factory method for instantiating a qubit.
        /// </summary>
        public Qubit MakeQubit(float xOffset) {
            GameObject qubit = Instantiate(
                _qubitPrefab, 
                Vector3.zero + new Vector3(xOffset, 0, 0), 
                _qubitPrefab.transform.rotation
            );

            return qubit.GetComponent<Qubit>();
        }

        /// <summary>
        /// Spawn the player. 
        /// TODO: Will need to add a way to know specific spawnpoints
        /// </summary>
        public Player SpawnPlayer() {
            GameObject player = Instantiate(
                _playerPrefab,
                Vector3.zero,
                _playerPrefab.transform.rotation
            );

            _isPlayerSpawned = true;
            return player.GetComponent<Player>();
        }
        #endregion Instantiation Methods
    }
}

