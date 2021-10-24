using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

using Managers;

[RequireComponent(typeof(WaveMoveScript))]
public sealed class EnemyWaveScript : MonoBehaviour
{
    private Func<Controllable, UniTask> _killControllableAction;
    public Func<Controllable, UniTask> KillControllableAction {
        set => _killControllableAction = value;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Controllable")) {
            _killControllableAction?.Invoke(other.GetComponent<Controllable>()).Forget();
            Destroy(gameObject);
        }
    }
}
