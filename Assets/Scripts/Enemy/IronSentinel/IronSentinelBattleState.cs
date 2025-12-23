using UnityEngine;

public class IronSentinelBattleState : EnemyBattleState
{
    private IronSentinel ironSentinel;

    private enum AttackKind
    {
        None,
        Melee,
        Dash
    }

    public IronSentinelBattleState(IronSentinel ironSentinel, StateMachine stateMachine, string animBoolName) : base(ironSentinel, stateMachine, animBoolName)
    {
        this.ironSentinel = ironSentinel;
    }

    protected override void TryStartAttack()
    {
        // baseの処理は雑魚用の記載なので、呼ばない
        // (attackDistanceで制御しないようにする)

        Transform player = ironSentinel.GetPlayerReference();
        if (player == null)
            return;

        AttackKind kind = DecideAttackKind(player);

        switch (kind)
        {
            case AttackKind.Melee:
                FaceToTarget();
                stateMachine.ChangeState(ironSentinel.meleeAttackState);
                break;

            case AttackKind.Dash:
                FaceToTarget();
                stateMachine.ChangeState(ironSentinel.dashAttackState);
                break;

            case AttackKind.None:
            default:
                // まだ距離が足りないので何もしない（追いかけ続ける）
                break;
        }
    }

    // どの攻撃を選ぶのか、決定する
    private AttackKind DecideAttackKind(Transform player)
    {
        float dx = player.position.x - enemy.transform.position.x;
        float distanceX = Mathf.Abs(dx);


        if (distanceX <= ironSentinel.meleeAttackDistance)
        {
            return AttackKind.Melee;
        }

        // 中距離〜のダッシュ攻撃範囲
        if (distanceX <= ironSentinel.dashAttackDistance)
        {
            return AttackKind.Dash;
        }

        return AttackKind.None;
    }
}
