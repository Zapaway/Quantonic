using Cysharp.Threading.Tasks;

using Managers;

namespace StateMachines.QSM {
    /// <summary>
    /// Allows the controllable to shoot.
    /// </summary>
    public class QSMState : IState
    {
        protected ControlManager _ctrlManager;
        protected SpawnManager _spawnManager;
        protected QSM _stateMachine;

        private int _currSpawnedWaves;
        protected int CurrSpawnedWaves => _currSpawnedWaves;

        public QSMState(ControlManager ctrlManager, SpawnManager spawnManager, QSM stateMachine)
        {
            _ctrlManager = ctrlManager;
            _spawnManager = spawnManager;
            _stateMachine = stateMachine;
        }

        public virtual async UniTask Enter() {
            await UniTask.Yield();

            _currSpawnedWaves = 0;
        } 
        public virtual async UniTask HandleInput() {            
            await UniTask.Yield();
        } 
        public virtual async UniTask LogicUpdate() {
            await UniTask.Yield();

            bool isAval = _currSpawnedWaves < ControlManager.Instance.CurrentControllable?.QubitCount;
            if (_ctrlManager.IsSpawnWaveTriggered() && isAval) {
                // spawn a wave using spawnmanager (use async unitask forget)
                _currSpawnedWaves++;
            }
        } 
        public virtual async UniTask PhysicsUpdate() {
            await UniTask.Yield();
        } 
        public virtual async UniTask Exit() {
            await UniTask.Yield();
        } 
    }
}
