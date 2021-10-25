using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CheckpointScript : MonoBehaviour
{
    [SerializeField] private Sprite _untouched;
    [SerializeField] private Sprite _touched;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Controllable")) {
            // trigger spawn method in spawn manager
        }
    }
}
