using System;
using UnityEngine;
using UnityEngine.Events;

using Managers;

[RequireComponent(typeof(BoxCollider2D))]
public class CheckpointScript : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _untouched;
    [SerializeField] private Sprite _touched;
    [SerializeField] private UnityEvent _eventsToDoWhenTouched;
    private bool _isTouched;
    
    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!_isTouched && other.CompareTag("Controllable") && other.gameObject.GetInstanceID() == StageControlManager.Instance.PlayerGameObjID) {
            _isTouched = true;
            _spriteRenderer.sprite = _touched;
            SpawnManager.Instance.SetCheckpoint(this, other.GetComponent<Player>());
            SoundManager.Instance.StageSounds.PlayCheckpointTouchedSFX();

            _eventsToDoWhenTouched?.Invoke();
        }
    }

    public void ResetCheckpointState() {
        if (_isTouched) {
            _isTouched = false;
            _spriteRenderer.sprite = _untouched;
        }
    }
}
