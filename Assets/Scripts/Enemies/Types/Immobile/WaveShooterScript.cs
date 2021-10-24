using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

using Managers;

public sealed class WaveShooterScript : ImmobileEnemy, IShooting, IDeath
{
    [SerializeField] private GameObject _enemyWavePrefab;
    [SerializeField] private GameObject _detectorObj;
    private ShootingDetector _detector;

    private GameObject _targetObj;
    [SerializeField] private float _cooldownSeconds;
    private float _secondsRemaining = 0;

    #region Unity Events
    protected override void Awake() {
        base.Awake();

        _detector = _detectorObj.GetComponent<ShootingDetector>();
        _detector.TriggeredEnterAction = _detectedAction;
        _detector.TriggeredExitAction = _stopAction;
    }

    private void Update() {
        if (_targetObj != null) {
            if (_secondsRemaining <= 0) {
                _secondsRemaining = _cooldownSeconds;
                ShootAction(_targetObj).Forget();
            }
            else _secondsRemaining -= Time.deltaTime;
        } 
    }
    #endregion Unity Events

    public async UniTask ShootAction(GameObject target) {
        await UniTask.Yield();

        float aimAng = GlobalControlManager.Instance.GetAngleRelativeToTarget(target.transform.position, transform.position);
        GameObject waveObj = Instantiate(
            _enemyWavePrefab,
            transform.position,
            Quaternion.Euler(0, 0, aimAng)
        );
        waveObj.GetComponent<EnemyWaveScript>().KillControllableAction = _kill;
    }
    public void DeathAction() {
        Destroy(gameObject);
    }

    /// <summary>
    /// When the wave has detected the current controllable, allow the enemy to keep shooting every n seconds.
    /// </summary>
    private void _detectedAction(Collider2D other) {
        if (_getIfCurrentControllableInRange(other)) _targetObj = other.gameObject;
    }
    /// <summary>
    /// Stop shooting when the controllable is out of range.
    /// </summary>
    private void _stopAction(Collider2D other) {
        if (StageControlManager.Instance.CurrentControllable == null || _getIfCurrentControllableInRange(other)) {
            _targetObj = null;
            _secondsRemaining = 0;
        }
    }
    private bool _getIfCurrentControllableInRange(Collider2D other) {
        return other.CompareTag("Controllable") && other.gameObject == StageControlManager.Instance.CurrentControllable?.gameObject;
    }
}
