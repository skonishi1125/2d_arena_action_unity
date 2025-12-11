using UnityEngine;

public class EnemyRangeAttackState : EnemyState
{
    public EnemyRangeAttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // 射撃方向などの調整が必要なら、EnterをOverrideして敵が各々書けばよい。
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (triggerCalled)
            stateMachine.ChangeState(enemy.battleState);
    }



}
