using UnityEngine;

public class PlayerDashState : PlayerState
{
    [Header("Dash Based Param")]
    // ダッシュ中は重力加速度を無効にする
    private float originalGravityScale;
    private int dashDir; // ダッシュ中の向き player側を使ってもいいが、保険でこっちを使っておく
    private bool hasAttack; // SLvが高く、攻撃判定がついているか


    public PlayerDashState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // 向き先、有効時間、重力加速度無効化設定
        dashDir = player.moveInput.x != 0 ? ((int)player.moveInput.x) : player.facingDir;
        stateTimer = player.dashDuration;
        originalGravityScale = rb.gravityScale; // 既存のrbを保存しておき、0にする
        rb.gravityScale = 0;

        hasAttack = player.Skill.DashHasStartAttack();
        if (hasAttack)
        {
            // 現レベル時点でのSkillデータを取得
            var levelData = player.Skill.GetCurrentLevelData(SkillId.Dash);
            if (levelData == null)
                return;

            // ダメージ倍率設定
            player.EntityCombat.SetDamageMultiplier(levelData.damageMultiplier);

            // KB設定
            player.EntityCombat.SetKnockback(
                levelData.knockbackPower,
                levelData.knockbackDuration
            );

            //Debug.Log($"[DashState]multi: {levelData.damageMultiplier} KBp: {levelData.knockbackPower} KBd: {levelData.knockbackDuration}");
        }

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        CancelDashIfNeeded();

        if (stateTimer < 0)
        {
            if (player.groundDetected)
                stateMachine.ChangeState(player.idleState);
            else
                stateMachine.ChangeState(player.fallState);

        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        player.SetVelocity(player.dashSpeed * dashDir, 0);
    }

    public override void Exit()
    {
        base.Exit();
        // DashStateから出るとき、慣性と重力加速度, ダメージ,KBを戻す
        player.SetVelocity(0, 0);
        rb.gravityScale = originalGravityScale;
        if (hasAttack)
        {
            player.EntityCombat.ResetDamageMultiplier();
            player.EntityCombat.ResetKnockback();
        }
    }

    // 壁にぶつかったときなどに、DashStateをキャンセルして別Stateに移行させる
    private void CancelDashIfNeeded()
    {
        if (player.wallDetected)
        {
            if (player.groundDetected)
                stateMachine.ChangeState(player.idleState);
            else
                stateMachine.ChangeState(player.wallSlideState);
        }
    }


}
