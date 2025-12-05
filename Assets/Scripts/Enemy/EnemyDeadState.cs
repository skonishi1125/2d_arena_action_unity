using UnityEngine;

public class EnemyDeadState : EnemyState
{
    public EnemyDeadState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        // animのsetbool起動処理を行わないので、baseを実行しない
        // base.Enter();
        anim.enabled = false;
        enemy.co.enabled = false;

        rb.gravityScale = 12f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 15);

        // 以降、deadStateから別の状態に遷移することはない。
        stateMachine.SwitchOffStateMachine();

        enemy.DiedDestroy();

    }

}
