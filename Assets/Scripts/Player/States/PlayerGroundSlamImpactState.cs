using UnityEngine;

public class PlayerGroundSlamImpactState : PlayerState
{
    public PlayerGroundSlamImpactState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }


    public override void Enter()
    {
        base.Enter();

        // 現レベル時点でのSkillデータを取得
        var levelData = player.Skill.GetCurrentLevelData(SkillId.GroundSlam);
        if (levelData == null)
            return;

        // ダメージ倍率設定
        player.EntityCombat.SetDamageMultiplier(levelData.damageMultiplier);

        // KB設定
        player.EntityCombat.SetKnockback(
            levelData.knockbackPower,
            levelData.knockbackDuration
        );

        // 横長の攻撃判定
        player.EntityCombat.SetHitboxBox(player.groundSlumCheck, player.groundSlamRange);

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }

    public override void Exit()
    {
        base.Exit();

        player.EntityCombat.ResetDamageMultiplier();
        player.EntityCombat.ResetKnockback();
        player.EntityCombat.ResetHitbox();
    }

}
