using UnityEngine;
using Cysharp.Threading.Tasks;

using Managers;

[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public abstract class Enemy : MonoBehaviour
{
    protected async UniTask _kill() {
        await StageControlManager.Instance.DestroyCurrentControllable();
    }
}
