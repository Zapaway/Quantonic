using System;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

public sealed class TimerScript : MonoBehaviour
{
    private TextMeshProUGUI _timerText;
    private const float _initalSeconds = 100;
    private float _currSeconds = _initalSeconds;

    // used for event-based handling
    private bool _isRunning = false; 
    private Func<UniTask> _onTimerRanOut;
    private async UniTask _invokeTimerRanOut() {
        if (_onTimerRanOut != null) await _onTimerRanOut();
    }

    private void Awake() {
        _timerText = GetComponent<TextMeshProUGUI>();

        ResetTimer();  // ensure that timer is set to init seconds
    }

    private async UniTaskVoid Update() {
        if (_isRunning) {
            _currSeconds -= Time.deltaTime;
            _setTimerText(_currSeconds);
            
            if (_currSeconds < 1) {
                StopTimer(keepTimerRunOutAction: true);
                _setTimerText(0);

                await _invokeTimerRanOut();

                ResetTimer();
            }
        }
    }

    #region Timer Actions
    /// <summary>
    /// Start the timer if it isn't running already.
    /// </summary>
    public void StartTimer(Func<UniTask> timerRunOutActionAsync = null) {
        if (!_isRunning) {
            _isRunning = true;
            
            _onTimerRanOut += async () => {
                if (timerRunOutActionAsync != null) await timerRunOutActionAsync();
            };
        }
    }
    private void _startTimer() {

    }

    /// <summary>
    /// Stop the timer if it isn't idle already.
    /// </summary>
    public void StopTimer(bool keepTimerRunOutAction = false) {
        if (_isRunning) {
            _isRunning = false;
            if (keepTimerRunOutAction == false) _onTimerRanOut = null;
        };
    }

    public void ResetTimer(bool keepTimerRunOutAction = false) {
        StopTimer(keepTimerRunOutAction);

        _currSeconds = _initalSeconds;
        _setTimerText(_currSeconds);
    }

    private void _setTimerText(float seconds) {
        _timerText.SetText($"{Mathf.FloorToInt(seconds)} µs");
    }
    #endregion Timer Actions
}
