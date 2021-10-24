using System;
using Cysharp.Threading.Tasks;

using Managers;

namespace StateMachines.QSM {
    /// <summary>
    /// Allows the controllable to shoot.
    /// </summary>
    public class QSMState : IState
    {
        protected StageControlManager _ctrlManager;
        protected StageUIManager _uiManager;
        protected SpawnManager _spawnManager;
        protected Controllable _controllable;
        protected QSM _stateMachine;

        private int _currSpawnedWaves;
        protected int CurrSpawnedWaves => _currSpawnedWaves;
        private bool _isNotOnCooldown = true;
        private const float _cooldownSec = 1.5f;

        public QSMState(
            StageControlManager ctrlManager, 
            StageUIManager stageUIManager,
            SpawnManager spawnManager, 
            Controllable controllable, 
            QSM stateMachine
        ) {
            _ctrlManager = ctrlManager;
            _uiManager = stageUIManager;
            _spawnManager = spawnManager;
            _controllable = controllable;
            _stateMachine = stateMachine;
        }

        public virtual async UniTask Enter() {
            await UniTask.Yield();

            _currSpawnedWaves = 0;
        } 
        public virtual async UniTask HandleInput() {            
            await UniTask.Yield();

            bool isAval = _isNotOnCooldown && (
                _controllable == null ? false : _currSpawnedWaves < _controllable.QubitCount
            );
            if (_ctrlManager.IsSpawnWaveTriggered() && isAval) {
                _spawnWave().Forget();
                _currSpawnedWaves++;
                
                // cooldown 
                _isNotOnCooldown = false;
                await UniTask.Delay(TimeSpan.FromSeconds(_cooldownSec));
                _isNotOnCooldown = true;
            }
        } 
        public virtual async UniTask LogicUpdate() {
            await UniTask.Yield();

            if (_controllable.QubitCount > 1) {
                await _stateMachine.ChangeState(_controllable.MultiState);
            }
        } 
        public virtual async UniTask PhysicsUpdate() {
            await UniTask.Yield();
        } 
        public virtual async UniTask Exit() {
            await UniTask.Yield();
        } 

        /// <summary>
        /// Spawn a wave. Once it is destroyed, allow the player to use their ability one more time.
        /// </summary>
        private async UniTaskVoid _spawnWave() {
            await UniTask.Yield();

            await _spawnManager.SpawnPlayerWave(_controllable.transform.position);
            _currSpawnedWaves--;
        }
    }
}
