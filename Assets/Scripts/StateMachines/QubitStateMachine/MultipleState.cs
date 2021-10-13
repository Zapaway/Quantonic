using Cysharp.Threading.Tasks;
using UnityEngine;

using Managers;

namespace StateMachines.QSM {
    public sealed class MultipleState : QSMState
    {
        // used for checking if the controllable can spawn its clones
        private bool _hasAvalSpace;
        private const float _spacingBetweenClones = 1;
        private readonly float _radiusOfChecking = _spacingBetweenClones;

        public MultipleState(
            StageControlManager ctrlManager, 
            StageUIManager stageUIManager,
            SpawnManager spawnManager, 
            Controllable controllable, 
            QSM stateMachine
            ) : base(ctrlManager, stageUIManager, spawnManager, controllable, stateMachine) 
        {
            _radiusOfChecking += controllable.transform.localScale.x * 2;  // multiplied since clones will need to be at least right next to the controllable
        }

        public override async UniTask Enter() {
            await base.Enter();
        } 
        public override async UniTask HandleInput() {            
            await base.HandleInput();

            // TODO: Check if the QQV panel is open
            if (_ctrlManager.IsSplitToggled() && _hasAvalSpace && !_controllable.IsBusy) {
                /*
                 * FILES TO OPEN
                 * StageControlManager
                 * SpawnManager
                 * StageUIManager
                 * Controllable
                 * QSMState
                 * QubitCircuit
                 * QubitSubcircuit
                 */

                // create clone
                Clone clone = _spawnManager.SpawnClone(_controllable, _spacingBetweenClones);

                // transfter qubit from curr qubitsubcirc to the clone's and update qubitcirc


                // reflect on stage ui
            }
        } 
        public override async UniTask LogicUpdate() {
            await base.LogicUpdate();

            if (_controllable.QubitCount < 1) {
                await _stateMachine.ChangeState(_controllable.QSMState);
            }
            else _hasAvalSpace = _isThereSpace();
        } 
        public override async UniTask PhysicsUpdate() {
            await base.PhysicsUpdate();
        } 
        public override async UniTask Exit() {
            await base.Exit();
        } 

        private bool _isThereSpace() {
            Collider2D collider = Physics2D.OverlapCircle(
                _controllable.transform.position,
                _radiusOfChecking,
                _ctrlManager.DefaultLayerMask
            );

            return collider == null;
        } 
    }
}
