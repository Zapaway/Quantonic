using System;
using System.Collections;
using UnityEngine;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(Rigidbody2D))]
/// <summary>
/// Scales while it moves in a certain direction before despawning.
/// TODO: Create a certain direction.
/// </summary>
public sealed class WaveMoveScript : MonoBehaviour
{
    private Rigidbody2D _waveRb;

    // movement
    private static float _moveSpeed = 5f;

    // scaling (shared properties)
    private static readonly float _duration = 5f;
    private static readonly float _initScale = 1f;
    private static readonly float _scaleAmp = 2f;
    private static readonly float _animationSeconds = 1f;
    private static readonly int _frames = 100;

    // scaling (non-shared properties)
    private float _currentDuration = 0f;
    private float _currentScale = _initScale;
    private float _deltaTime = _animationSeconds/_frames; 
    private float _deltaScale = _scaleAmp/_frames;

    private void Awake() {
        _waveRb = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        _moveAndScale().Forget();
        _waveRb.velocity = new Vector2(_moveSpeed, _waveRb.velocity.y);
    }

    private void Update() {
        _currentDuration += Time.deltaTime;
        if (_currentDuration > _duration) {
            Destroy(gameObject);
        }
    }

    private async UniTaskVoid _moveAndScale() {
        while (true) {
            _currentScale += _deltaScale;
            if (this != null) transform.localScale = Vector3.one * _currentScale;
            await UniTask.Delay(TimeSpan.FromSeconds(_deltaTime));
        }
    }
}
