using UnityEngine;

using Managers;

[RequireComponent(typeof(BoxCollider2D))]
public class CheckpointScript : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _untouched;
    [SerializeField] private Sprite _touched;
    private bool _isTouched;
    
    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!_isTouched && other.CompareTag("Controllable") && other.gameObject.GetInstanceID() == StageControlManager.Instance.PlayerID) {
            _isTouched = true;
            _spriteRenderer.sprite = _touched;
            SpawnManager.Instance.SetCheckpoint(this, other.GetComponent<Player>());
        }
    }

    public void ResetCheckpointState() {
        if (_isTouched) {
            _isTouched = false;
            _spriteRenderer.sprite = _untouched;
        }
    }
}
