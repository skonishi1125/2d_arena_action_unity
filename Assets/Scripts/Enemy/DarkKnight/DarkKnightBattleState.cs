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
        base.TryStartAttack();

        Transform player = enemy.GetPlayerReference();
        if (player == null)
            return;

        float distance = Mathf.Abs(player.position.x - enemy.transform.position.x);

        // 例：まずはダッシュ攻撃に入る条件だけ作る
        if (distance <= darkKnight.dashAttackDistance)
        {
            stateMachine.ChangeState(darkKnight.dashAttackState);
        }

        // 近距離斬りを作るときはここに条件を追加
        // else if (distance <= meleeDistance) { ... }
    }


}
