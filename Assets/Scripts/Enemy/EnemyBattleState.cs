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

        // 感知から状態に入ったケース, 後ろからplayerに殴られたケースなどを考慮
        player ??= enemy.GetPlayerReference(); // if(player == null) player = ...と同じ

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        TryStartAttack();
    }

    // virtualとして、DarkKnightなどでオーバーライドできるようにする
    protected virtual void TryStartAttack()
    {
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
    protected bool WithinAttackRange()
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
