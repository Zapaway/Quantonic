using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

using Quantum;
using Managers;

public sealed class Player : Controllable
{
    protected override async UniTask Start()
    {
        await base.Start();

        _addInitQubits();
        SpawnManager.Instance.SaveLocalState(this);
    }

    /// <summary>
    /// Actives the player and adds the qubits if it is already deactivated. 
    /// </summary>
    public void Activate(QuantumState[] states = null) {
        if (!gameObject.activeSelf) {
            gameObject.SetActive(true);

            if (states == null) _addInitQubits();
            else _addPrevQubits(states);
        }
    }

    /// <summary>
    /// Deactivates the player and clear the qubits if it is already activated.
    /// </summary>
    public void Deactivate() {
        if (gameObject.activeSelf) {
            gameObject.SetActive(false);
            SoundManager.Instance.StageSounds.PlayExplosionSFX();
            _clear();
        }
    }

    private void _addInitQubits() {
        _addQubit();
        _test();
    }
    private void _addPrevQubits(QuantumState[] states) {
        foreach (var state in states) {
            _addQubit(state);
        }
    }

    private void _test() {
        _addQubit();
        _addQubit();
        _addQubit();
        // _addQubit();
    }
}
