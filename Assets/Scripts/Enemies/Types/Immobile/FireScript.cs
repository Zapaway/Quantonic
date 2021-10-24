using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public sealed class FireScript : ImmobileEnemy, ICollision
{
    public async UniTask TouchedAction(GameObject gameObject) {
        await _kill(gameObject.GetComponent<Controllable>());
    }

    private async UniTaskVoid OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Controllable")) {
            await TouchedAction(other.gameObject);
        }
    }
}
