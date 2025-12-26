using UnityEngine;

public class PlayerKnockbackAttackState : PlayerState
{
    public PlayerKnockbackAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        triggerCalled = false; // triggerの残留を防ぐ
        base.Enter();

        // 現レベル時点でのSkillデータを取得
        var levelData = player.Skill.GetCurrentLevelData(SkillId.HeavyKB);
        if (levelData == null)
            return;

        // ダメージ倍率設定
        player.EntityCombat.SetDamageMultiplier(levelData.damageMultiplier);

        // KB設定
        player.EntityCombat.SetKnockback(
            levelData.knockbackPower,
            levelData.knockbackDuration
        );

        //Debug.Log($"[KnockbackAttackState] multi: {levelData.damageMultiplier} KBp: {levelData.knockbackPower} KBd: {levelData.knockbackDuration}");

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // 攻撃アニメ中に滑ってしまうのを防ぐ
        player.SetVelocity(0, rb.linearVelocity.y);
    }

    public override void Exit()
    {
        base.Exit(); // これで anim.SetBool(false) を呼んでいる
        player.EntityCombat.ResetDamageMultiplier();
        player.EntityCombat.ResetKnockback();
    }
}
