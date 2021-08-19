using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers {
    /// <summary>
    /// This is a global manager.
    /// </summary>
    [RequireComponent(typeof(AddOns.DontDestroyOnLoad))]
    public class GameManager : Manager<GameManager>
    {
        protected override void Awake()
        {
            base.Awake();
        }
    }
}
