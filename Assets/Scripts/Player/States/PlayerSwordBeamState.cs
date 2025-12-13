using UnityEngine;

public class PlayerSwordBeamState : PlayerState
{
    private float attackVelocityTimer;

    public PlayerSwordBeamState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        attackVelocityTimer = player.attackVelocityDuration;

        // スキルSOからダメージ倍率とKB情報の取得
        var levelData = player.Skill.GetCurrentLevelData(SkillId.SwordBeam);
        if (levelData == null)
            return;

        // player.EntityProjectile.Set...とはできない
        // (playerはProjectileSpawnerしか持っておらず、Projectileの参照ができない)
        // なのでスポナーにスキルダメージ情報を送る。
        var ctx = new ProjectileDamageContext
        {
            damageMultiplier = levelData.damageMultiplier,
            hasCustomKnockback = true,
            knockbackPower = levelData.knockbackPower,
            knockbackDuration = levelData.knockbackDuration,
        };

        // Player側に弾情報をセット
        // トリガー側で、Spawn()させるときにPlayerから参照できるようにしておく
        player.SetPendingProjectileCtx(ctx);

        Debug.Log($"[SwordBeam] multi: {levelData.damageMultiplier} KBp: {levelData.knockbackPower} KBd: {levelData.knockbackDuration}");

    }


    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (triggerCalled)
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
        HandleAttackVelocity();
    }

    private void HandleAttackVelocity()
    {
        // たとえば.1fの時、0.1fの間だけ左右入力で滑る。
        // -1で固定すると全く滑らなくなり、
        // この処理自体をなくすと攻撃の間、移動速度で滑り続ける。
        attackVelocityTimer -= Time.deltaTime;
        if (attackVelocityTimer < 0)
            player.SetVelocity(0, rb.linearVelocity.y);
    }

}
