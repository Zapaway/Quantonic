using Cysharp.Threading.Tasks;

using Managers;

namespace StateMachines.QSM {
    public abstract class QSMState : IState
    {
        protected ControlManager _ctrlManager;
        protected QSM _stateMachine;


        public QSMState(QSM stateMachine)
        {
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
