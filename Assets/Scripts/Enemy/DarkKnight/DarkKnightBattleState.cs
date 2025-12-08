using UnityEngine;

public class DarkKnightBattleState : EnemyBattleState
{
    private DarkKnight darkKnight;
    public DarkKnightBattleState(DarkKnight enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        darkKnight = enemy;
    }

    protected override void TryStartAttack()
    {
        // baseの処理は雑魚用の記載なので、呼ばない
        // (attackDistanceで制御しないようにする)
        //base.TryStartAttack();

        Transform player = darkKnight.GetPlayerReference();
        if (player == null)
            return;

        // 例：まずはダッシュ攻撃に入る条件だけ作る
        if (WithinAttackRange())
        {
            stateMachine.ChangeState(darkKnight.dashAttackState);
        }

        // 近距離斬りを作るときはここに条件を追加
        // else if (distance <= meleeDistance) { ... }
    }

    protected override bool WithinAttackRange()
    {
        // 先に近距離を判定(予定)

        // その後、ダッシュを判定
        // とすることで、なんとかなりそう
        // BoolじゃなくEnumで結果を返して、ChangeStateはSwitchで分岐させるとか？
        return DistanceToPlayer() < darkKnight.dashAttackDistance;
    }
}
