using System;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

public sealed class TimerScript : MonoBehaviour
{
    private TextMeshProUGUI _timerText;
    private const float _initalSeconds = 100;
    private float _currSeconds = _initalSeconds + 1;

    // used for event-based handling
    private bool _isRunning = false; 
    private Func<UniTask> _onTimerRanOut;
    private async UniTask _invokeTimerRanOut() {
        if (_onTimerRanOut != null) await _onTimerRanOut();
    }

    private int _testCount = 0;

    private void Awake() {
        _timerText = GetComponent<TextMeshProUGUI>();
        _setTimerText(_currSeconds);  // ensure that timer is set to init seconds
    }

    private async UniTaskVoid Update() {
        if (_isRunning) {
            _currSeconds -= Time.deltaTime;
            _setTimerText(_currSeconds);
            
            if (_currSeconds < 1) {
                PauseTimer();
                _setTimerText(0);

                await _invokeTimerRanOut();
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
            
            if (timerRunOutActionAsync != null) {
                _onTimerRanOut += async () => await timerRunOutActionAsync();
                _testCount++;
            }
        }
    }

    /// <summary>
    /// Pause the timer if it isn't idle already. Keep the time out action. 
    /// </summary>
    public void PauseTimer() {
        if (_isRunning) _isRunning = false;
    }

    /// <summary>
    /// Reset the timer.
    /// </summary>
    public void ResetTimer(bool keepTimerRunOutAction = false) {
        PauseTimer();
        if (!keepTimerRunOutAction) _resetTimerRunOutAction(); 

        _currSeconds = _initalSeconds + 1;
        _setTimerText(_currSeconds);
    }

    private void _setTimerText(float seconds) {
        _timerText.SetText($"{Mathf.FloorToInt(seconds)} µs");
    }

    private void _resetTimerRunOutAction() {
        _onTimerRanOut = null;
    }

    #endregion Timer Actions
}
