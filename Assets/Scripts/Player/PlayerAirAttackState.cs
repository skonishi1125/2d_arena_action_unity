using UnityEngine;

public class PlayerAirAttackState : PlayerAirState
{
    // 空中攻撃中は、重力加速度を和らげる
    private float originalGravityScale;
    public PlayerAirAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        originalGravityScale = rb.gravityScale; // 既存のrbを保存しておき、0にする
        rb.gravityScale = 0f;

        rb.linearVelocity = Vector2.zero; // 空中攻撃したとき、慣性を完全に止める

        // ダメージ倍率設定
        float dmgMul = player.airAttackDamageMultiplier;
        player.EntityCombat.SetDamageMultiplier(dmgMul);

        // KB設定
        Vector2 kbPower = player.airAttackKnockbackPower;
        player.EntityCombat.SetKnockback(kbPower, player.airAttackKnockbackDuration);

    }

    public override void PhysicsUpdate()
    {
        // 親のPhysicsUpdateは稼働させない（攻撃中の移動受付を止める）
        // base.PhysicsUpdate();

        // 攻撃中に着地したら、Idle
        if(player.groundDetected)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }

        // 縦速度の制御 ちょっとずつ下がっていく
        float currentVy = rb.linearVelocity.y;
        float targetVy = player.AirAttackFallSpeed;
        float newVy = Mathf.MoveTowards(
            currentVy,
            targetVy,
            player.AirAttackVerticalAccel * Time.fixedDeltaTime
        );

        // xを0にすることで、ジャンプ前の慣性や攻撃中の入力どちらも無効化する
        rb.linearVelocity = new Vector2(0f, newVy);

        if (triggerCalled)
            stateMachine.ChangeState(player.fallState);

    }

    public override void Exit()
    {
        base.Exit();
        rb.gravityScale = originalGravityScale;
    }



}
