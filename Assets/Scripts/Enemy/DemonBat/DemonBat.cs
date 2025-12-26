using UnityEngine;

public class DemonBat : Enemy
{
    // 2つ目の壁判定
    [SerializeField] private Transform secondWallCheck;

    protected override void Awake()
    {
        base.Awake();

        // この割当はEnemy側ではやらない。
        // Enemy側で行うと、thisに入れる対象がどのモンスターなのか判断できなくなる
        idleState = new EnemyIdleState(this, stateMachine, "idle");
        moveState = new EnemyMoveState(this, stateMachine, "move");
        battleState = new EnemyBattleState(this, stateMachine, "battle");
        attackState = new EnemyAttackState(this, stateMachine, "attack");
        //// deadに入る時は、dead前のアニメーションを使う
        deadState = new EnemyDeadState(this, stateMachine, "NONE");

    }


    protected override void HandleCollisionDetection()
    {
        base.HandleCollisionDetection();
        wallDetected = Physics2D.Raycast(
            secondWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround
        );
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        // 壁
        Gizmos.DrawLine(
            secondWallCheck.position,
            secondWallCheck.position + new Vector3(wallCheckDistance * facingDir, 0)
        );
    }
}
