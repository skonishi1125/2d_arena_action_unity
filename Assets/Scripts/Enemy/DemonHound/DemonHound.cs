using UnityEngine;

public class DemonHound : Enemy
{
    protected override void Awake()
    {
        base.Awake();

        // この割当はEnemy側ではやらない。
        // Enemy側で行うと、thisに入れる対象がどのモンスターなのか判断できなくなる
        idleState = new EnemyIdleState(this, stateMachine, "idle");
        moveState = new EnemyMoveState(this, stateMachine, "move");
        battleState = new EnemyBattleState(this, stateMachine, "battle");
        attackState = new EnemyAttackState(this, stateMachine, "attack");

    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState); // 初期状態の設定 + 入口処理

    }

}
