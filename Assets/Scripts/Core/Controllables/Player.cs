using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public sealed class Player : Controllable
{
    private float xTracker = 0;
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start() {
        _addQubitFromPrefab(xTracker);  // all players should start off with one qubit
        Testing().Forget();
    }

    private async UniTaskVoid Testing() {
        while (true) {
            await UniTask.Delay(TimeSpan.FromSeconds(5));
            xTracker += 5;
            _addQubitFromPrefab(xTracker);
        }
    }
}
