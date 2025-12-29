
using System;
using UnityEngine;

public class StateMachine
{
    // このクラスの役割は、現状態が何かという参照を出すこと。
    // しかしstateが勝手に変更されてはいけないので、private setとする。
    public EntityState currentState { get; private set; }
    public bool canChangeState; // dead時などに切って、以降stateを不変に保つ

    public bool logTransitions = false;
    public bool logStackTrace = false;
    private int seq = 0;


    // ゲーム開始時など、初期stateを割り当てるためのメソッド
    public void Initialize(EntityState startState)
    {
        canChangeState = true;
        currentState = startState;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (logTransitions)
        {
            Debug.Log($"[SM] init frame={Time.frameCount} state={StateName(currentState)}");
        }
#endif

        currentState.Enter(); // 入口処理
    }

    // 現状態を変更するためのメソッド
    public void ChangeState(EntityState newState, string reason = null)
    {
        if (!canChangeState)
            return;

        if (newState == null)
            return;

        // 同一ステートへの遷移を無視すると、ジャンプなどで支障が出る
        //if (ReferenceEquals(currentState, newState))
        //    return;

        var from = currentState;
        var to = newState;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (logTransitions)
        {
            seq++;

            string r = string.IsNullOrEmpty(reason) ? "" : $" reason={reason}";
            Debug.Log(
                $"[SM] #{seq} frame={Time.frameCount} t={Time.time:F3} dt={Time.deltaTime:F4} " +
                $"{StateName(from)} -> {StateName(to)}{r}"
            );

            if (logStackTrace)
            {
                // 重いので、再現時だけON
                Debug.Log(Environment.StackTrace);
            }
        }
#endif


        from?.Exit();
        currentState = to;
        to.Enter();

        //currentState.Exit(); // 出口処理
        //currentState = newState;
        //currentState.Enter(); // 新しいStateの入口処理
    }

    public void SwitchOffStateMachine()
    {
        canChangeState = false;
    }

    private static string StateName(EntityState s)
    {
        return s == null ? "null" : s.GetType().Name;
    }

}
