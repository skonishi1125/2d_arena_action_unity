using UnityEngine;

public class DarkKnightBattleState : EnemyBattleState
{
    private DarkKnight darkKnight;

    private enum AttackKind
    {
        None,
        Melee,
        Dash
    }

    public DarkKnightBattleState(DarkKnight enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        darkKnight = enemy;
    }

    protected override void TryStartAttack()
    {
        // baseの処理は雑魚用の記載なので、呼ばない
        // (attackDistanceで制御しないようにする)

        Transform player = darkKnight.GetPlayerReference();
        if (player == null)
            return;

        AttackKind kind = DecideAttackKind(player);

        switch (kind)
        {
            case AttackKind.Melee:
                FaceToPlayer(player);
                stateMachine.ChangeState(darkKnight.meleeAttackState);
                break;

            case AttackKind.Dash:
                FaceToPlayer(player);
                stateMachine.ChangeState(darkKnight.dashAttackState);
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


        if (distanceX <= darkKnight.meleeAttackDistance)
        {
            return AttackKind.Melee;
        }

        // 中距離〜のダッシュ攻撃範囲
        if (distanceX <= darkKnight.dashAttackDistance)
        {
            return AttackKind.Dash;
        }

        return AttackKind.None;
    }

    private void FaceToPlayer(Transform player)
    {
        float dx = player.position.x - enemy.transform.position.x;
        if (dx > 0 && enemy.facingDir < 0)
            enemy.Flip();
        else if (dx < 0 && enemy.facingDir > 0)
            enemy.Flip();
    }


}
