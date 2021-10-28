using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(Collider2D))]
public sealed class ShootingDetector : MonoBehaviour
{
    private Action<Collider2D> _triggeredEnterAction;
    public Action<Collider2D> TriggeredEnterAction {
        set => _triggeredEnterAction = value;
    }

    private Action<Collider2D> _triggeredExitAction;
    public Action<Collider2D> TriggeredExitAction {
        set => _triggeredExitAction = value;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        _triggeredEnterAction(other);
    }

    private void OnTriggerExit2D(Collider2D other) {
        _triggeredExitAction(other);
    }
}
