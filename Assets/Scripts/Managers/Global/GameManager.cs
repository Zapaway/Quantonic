using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers {
    /// <summary>
    /// Handles all game information and events. 
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
