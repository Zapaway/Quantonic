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
        Qubit qubit0 = _addQubitFromPrefab(xTracker);  // all players should start off with one qubit
        qubit0.name = "qubit0";
        // Testing().Forget();
        // Testing2().Forget();
    }

    private async UniTaskVoid Testing() {
        while (true) {
            await UniTask.Delay(TimeSpan.FromSeconds(5));
            xTracker += 5;
            _addQubitFromPrefab(xTracker);
        }
    }
    private async UniTaskVoid Testing2() {
        /// add
        xTracker += 5;
        Qubit qubit1 = _addQubitFromPrefab(xTracker);
        qubit1.name = "qubit1";
        xTracker += 5;
        Qubit qubit2 = _addQubitFromPrefab(xTracker);
        qubit2.name = "qubit2";
        xTracker += 5;
        Qubit qubit3 = _addQubitFromPrefab(xTracker);
        qubit3.name = "qubit3";

        // /// remove
        // await UniTask.Delay(TimeSpan.FromSeconds(2));
        // _subcirc.RemoveAt(3, isQCIndex: false);  // qubit3

        // // /// replace 
        // // await UniTask.Delay(TimeSpan.FromSeconds(2));
        // // Qubit newQubit2 = _addQubitFromPrefab(qubit2.transform.position); 
        // // newQubit2.name = "meow";
        // // Destroy(qubit2.gameObject);
        // // _subcirc[2] = newQubit2;

        // // /// move
        // // await UniTask.Delay(TimeSpan.FromSeconds(2));
        // // _subcirc.Move(0, 1);

        // /// reset
        // await UniTask.Delay(TimeSpan.FromSeconds(2));
        // _subcirc.Clear();
    }
}
