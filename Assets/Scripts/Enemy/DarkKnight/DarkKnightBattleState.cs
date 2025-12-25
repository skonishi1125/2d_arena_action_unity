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

    public DarkKnightBattleState(DarkKnight darkKnight, StateMachine stateMachine, string animBoolName) : base(darkKnight, stateMachine, animBoolName)
    {
        this.darkKnight = darkKnight;
    }

    protected override void TryStartAttack()
    {
        // baseの処理は雑魚用の記載なので、呼ばない
        // (attackDistanceで制御しないようにする)

        //Transform player = darkKnight.GetPlayerReference();
        if (target == null)
            return;

        AttackKind kind = DecideAttackKind(target);

        switch (kind)
        {
            case AttackKind.Melee:
                FaceToTarget();
                stateMachine.ChangeState(darkKnight.meleeAttackState);
                break;

            case AttackKind.Dash:
                FaceToTarget();
                stateMachine.ChangeState(darkKnight.dashAttackState);
                break;

            case AttackKind.None:
            default:
                // まだ距離が足りないので何もしない（追いかけ続ける）
                break;
        }
    }

    // どの攻撃を選ぶのか、決定する
    private AttackKind DecideAttackKind(Transform target)
    {
        float dx = target.position.x - enemy.transform.position.x;
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



}
