using UnityEngine;

public class PlayerMagicBoltState : PlayerState
{
    private float attackVelocityTimer;

    public PlayerMagicBoltState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        attackVelocityTimer = player.attackVelocityDuration;

        // スキルSOからダメージ倍率とKB情報の取得
        var levelData = player.Skill.GetCurrentLevelData(SkillId.MagicBolt);
        if (levelData == null)
            return;

        var state = player.Skill.GetState(SkillId.MagicBolt);
        if (state == null)
            return;

        var req = new ProjectileSpawnRequest
        {
            prefab = state.definition.projectilePrefab,
            damage =
            {
                damageMultiplier = levelData.damageMultiplier,
                hasCustomKnockback = true,
                knockbackPower = levelData.knockbackPower,
                knockbackDuration = levelData.knockbackDuration,
            },
            speedOverride = 20f, // todo: 必要ならスキルに持たせる
            destroyOnGround = true,
            destroyOnHit = true,
        };

        player.SetPendingProjectileRequest(req);

        Debug.Log($"[MagicBoltState] multi: {levelData.damageMultiplier} KBp: {levelData.knockbackPower} KBd: {levelData.knockbackDuration}");

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

    // 攻撃中の動きの制御
    private void HandleAttackVelocity()
    {
        // たとえば.1fの時、0.1fの間だけ左右入力で滑る。
        // -1で固定すると全く滑らなくなり、
        // この処理自体をなくすと攻撃の間、移動速度で滑り続ける。
        attackVelocityTimer -= Time.deltaTime;
        if (attackVelocityTimer < 0)
            player.SetVelocity(0, rb.linearVelocity.y);
    }

    public override void Exit()
    {
        base.Exit();

        // ここもどうしよう？
        // 弾はDestroyで消えてしまうので、不要かも
        // 今後ObjectPoolとしたら、
        // 使いまわすことになるのでその際にリセットが必要になりそう
        //player.EntityProjectile.ResetDamageMultiplier();
        //player.EntityProjectile.ResetKnockback();
    }

}
