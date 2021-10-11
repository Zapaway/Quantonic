using Cysharp.Threading.Tasks;

using Managers;

namespace StateMachines.QSM {
    public sealed class MultipleState : QSMState
    {
        public MultipleState(StageControlManager ctrlManager, SpawnManager spawnManager, Controllable controllable, QSM stateMachine) : base(ctrlManager, spawnManager, controllable, stateMachine) {}

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
