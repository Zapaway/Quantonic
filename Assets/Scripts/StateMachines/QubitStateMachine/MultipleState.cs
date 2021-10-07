using Cysharp.Threading.Tasks;

using Managers;

namespace StateMachines.QSM {
    public sealed class MultipleState : QSMState
    {
        public MultipleState(ControlManager ctrlManager, SpawnManager spawnManager, QSM stateMachine) : base(ctrlManager, spawnManager, stateMachine) {}

        public override async UniTask Enter() {
            await base.Enter();
        } 
        public override async UniTask HandleInput() {            
            await base.HandleInput();
        } 
        public override async UniTask LogicUpdate() {
            await base.LogicUpdate();
        } 
        public override async UniTask PhysicsUpdate() {
            await base.PhysicsUpdate();
        } 
        public override async UniTask Exit() {
            await base.Exit();
        } 
    }
}
