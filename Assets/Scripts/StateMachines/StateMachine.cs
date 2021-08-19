using Cysharp.Threading.Tasks;

namespace StateMachines {
    /// <summary>
    /// Base class for all singular state machines.
    /// </summary>
    public abstract class StateMachine<T> where T : IState
    {
        protected T _currState;

        /// <value>
        /// Current state of the state machine.
        /// </value>
        public T CurrentState => _currState;

        /// <summary>
        /// Initalize the state.
        /// </summary>
        public async virtual UniTask InitializeState(T initState) {
            _currState = initState;
            await _currState.Enter();
        }

        /// <summary>
        /// Change the state.
        /// </summary>
        public async virtual UniTask ChangeState(T newState) {
            await _currState.Exit();

            _currState = newState;
            await _currState.Enter();
        }
    }
}


