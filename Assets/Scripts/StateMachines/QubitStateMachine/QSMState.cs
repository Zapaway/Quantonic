using Cysharp.Threading.Tasks;

namespace StateMachines.QSM {
    public abstract class QSMState : IState
    {
        protected Controllable _controllable;
        protected QSM _stateMachine;

        public QSMState(Controllable controllable, QSM stateMachine)
        {
            _controllable = controllable;
            _stateMachine = stateMachine;
        }

        public virtual async UniTask Enter() {
            await UniTask.Yield();
        } 
        public virtual async UniTask HandleInput() {
            await UniTask.Yield();
        } 
        public virtual async UniTask LogicUpdate() {
            await UniTask.Yield();
        } 
        public virtual async UniTask PhysicsUpdate() {
            await UniTask.Yield();
        } 
        public virtual async UniTask Exit() {
            await UniTask.Yield();
        } 
    }
}
