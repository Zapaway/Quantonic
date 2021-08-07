using Cysharp.Threading.Tasks;

namespace StateMachines {
    /// <summary>
    /// Base class for all state machines.
    /// </summary>
    public abstract class StateMachine<T> where T : IState
    {
        /// <value>
        /// Current state of the state machine.
        /// </value>
        public abstract T CurrentState {get;}

        /// <summary>
        /// Initalize the state.
        /// </summary>
        public abstract UniTask InitializeState(T initState);
        /// <summary>
        /// Change the state.
        /// </summary>
        public abstract UniTask ChangeState(T newState);
    }
}


