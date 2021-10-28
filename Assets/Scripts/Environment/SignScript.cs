using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public sealed class SignScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _signText;

    private void Awake() {
        _signText.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Controllable")) _signText.enabled = true;
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Controllable")) _signText.enabled = false;
    }
}
