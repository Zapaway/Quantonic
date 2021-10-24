using UnityEngine;
using Cysharp.Threading.Tasks;

using Managers;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : MonoBehaviour
{
    protected PolygonCollider2D _polyCollider2D;
    public PolygonCollider2D PolyCollider2D => _polyCollider2D;
    protected Rigidbody2D _rigidbody2D;
    public Rigidbody2D Rigidbody2D => _rigidbody2D;

    protected virtual void Awake() {
        _polyCollider2D = GetComponent<PolygonCollider2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    protected async UniTask _kill(Controllable controllable) {
        await StageControlManager.Instance.DestroyControllable(controllable);
    }
}
