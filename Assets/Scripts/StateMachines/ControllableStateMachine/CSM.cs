using Cysharp.Threading.Tasks;

namespace StateMachines.CSM {
    /// <summary>
    /// Handles movement state of controllables. 
    /// There should only be one of these.
    /// All states can be disabled via disabling specific stage input.
    /// </summary>
    public sealed class CSM : StateMachine<CSMState> {}
}
