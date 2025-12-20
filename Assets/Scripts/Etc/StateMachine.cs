public class StateMachine
{
    // このクラスの役割は、現状態が何かという参照を出すこと。
    // しかしstateが勝手に変更されてはいけないので、private setとする。
    public EntityState currentState { get; private set; }
    public bool canChangeState; // dead時などに切って、以降stateを不変に保つ

    // ゲーム開始時など、初期stateを割り当てるためのメソッド
    public void Initialize(EntityState startState)
    {
        canChangeState = true;
        currentState = startState;
        currentState.Enter(); // 入口処理
    }

    // 現状態を変更するためのメソッド
    public void ChangeState(EntityState newState)
    {
        if (! canChangeState)
            return;

        currentState.Exit(); // 出口処理
        currentState = newState;
        currentState.Enter(); // 新しいStateの入口処理
    }

    public void SwitchOffStateMachine()
    {
        canChangeState = false;
    }

}
