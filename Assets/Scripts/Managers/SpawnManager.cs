using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers {
    /// <summary>
    /// Handles all spawning in a stage. Also contains prefabs for instantiation.
    /// </summary>
    public class SpawnManager : Manager<SpawnManager>
    {
        [SerializeField] private GameObject _qubitPrefab;
        public GameObject QubitPrefab => _qubitPrefab;

        protected override void Awake()
        {
            base.Awake();
        }

        #region Static Instantiate Methods
        /// <summary>
        /// Factory method for instantiating a qubit.
        /// </summary>
        public static Qubit MakeQubit(float xOffset) {
            GameObject qubitPrefab = SpawnManager.Instance.QubitPrefab;
            GameObject qubit = Instantiate(
                qubitPrefab, 
                Vector3.zero + new Vector3(xOffset, 0, 0), 
                qubitPrefab.transform.rotation
            );

            var qubitScript = qubit.GetComponent<Qubit>();
            return qubitScript;
        }
        #endregion Static Instantiate Methods
    }
}

