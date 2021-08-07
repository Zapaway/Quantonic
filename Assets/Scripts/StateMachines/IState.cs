using Cysharp.Threading.Tasks;

namespace StateMachines {
    /// <summary>
    /// All states must have these methods. Inspired by
    /// <see href="https://www.raywenderlich.com/6034380-state-pattern-using-unity#c-rate"></see>.
    /// </summary>
    public interface IState {
        UniTask Enter();
        UniTask HandleInput();
        UniTask LogicUpdate();
        UniTask PhysicsUpdate();
        UniTask Exit();
    }
}
