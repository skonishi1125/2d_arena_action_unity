using UnityEngine;

public class EnemyBattleState : EnemyState
{
    private Transform player;
    public EnemyBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        if (player == null)
            player = enemy.PlayerDetection().transform; // 感知したRaycastのtransform
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (WithinAttackRange())
        {
            stateMachine.ChangeState(enemy.attackState);
        }

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        if (!WithinAttackRange())
        {
            enemy.SetVelocity(enemy.battleMoveSpeed * DirectionToPlayer(), rb.linearVelocity.y);
        }
    }


    // Playerとの距離が、攻撃範囲より小さくなったらtrueを返す
    private bool WithinAttackRange()
    {
        return DistanceToPlayer() < enemy.attackDistance;
    }

    // 絶対値で、playerのx座標 - 敵のx座標の結果を返す
    private float DistanceToPlayer()
    {
        // 感知できない場合は、遠い位置にいるため最大値を返しておく
        if (player == null)
            return float.MaxValue;

        return Mathf.Abs(player.position.x - enemy.transform.position.x);
    }

    private int DirectionToPlayer()
    {
        if (player == null)
            return 0;

        if (player.position.x > enemy.transform.position.x)
        {
            return 1;
        }
        else
        {
            return -1;
        }

    }

}
