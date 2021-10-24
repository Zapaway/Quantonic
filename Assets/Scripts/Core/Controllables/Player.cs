using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public sealed class Player : Controllable
{
    protected override async UniTask Start()
    {
        await base.Start();

        _addInitQubits();
    }

    /// <summary>
    /// Actives the player and adds the qubits if it is already deactivated. 
    /// </summary>
    public void Activate() {
        if (!gameObject.activeSelf) {
            gameObject.SetActive(true);
            _addInitQubits();
        }
    }

    /// <summary>
    /// Deactivates the player and clear the qubits if it is already activated.
    /// </summary>
    public void Deactivate() {
        if (gameObject.activeSelf) {
            gameObject.SetActive(false);
            _clear();
        }
    }

    private void _addInitQubits() {
        _addQubit();
        _test();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        // // TODO: add conditoinal here to see if the stage ended or not
        // throw new NotSupportedException("Since the stage was not complete, you must use enabling to hide the player.");
    }

    private void _test() {
        _addQubit();
        _addQubit();
        _addQubit();
        // _addQubit();
    }


    // private async UniTaskVoid Testing2() {
    //     // /// remove
    //     // await UniTask.Delay(TimeSpan.FromSeconds(2));
    //     // _subcirc.RemoveAt(3, isQCIndex: false);  // qubit3

    //     // /// replace 
    //     // await UniTask.Delay(TimeSpan.FromSeconds(2));
    //     // Qubit newQubit2 = _addQubitFromPrefab(qubit2.transform.position); 
    //     // newQubit2.name = "meow";
    //     // Destroy(qubit2.gameObject);
    //     // _subcirc[2] = newQubit2;

    //     // /// move
    //     // await UniTask.Delay(TimeSpan.FromSeconds(2));
    //     // _subcirc.Move(0, 1);

    //     // /// reset
    //     // await UniTask.Delay(TimeSpan.FromSeconds(2));
    //     // _subcirc.Clear();
    // }
}
