using UnityEngine;

public class PlayerBasicAttackState : PlayerState
{
    // 攻撃中、前に進める猶予時間
    private float attackVelocityTimer;
    public PlayerBasicAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        GenerateAttackVelocity();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 攻撃モーションの終わりにアニメーショントリガーとして以下を挟む。
        // EntityState.CallAnimationTrigger()
        // trueとなった場合、攻撃stateから抜け出し、idleに戻る。
        // idleに戻ったとき(Enter時)にはfalseとなり,再びこの値が使えるようになる。
        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);
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

    // 攻撃中、向いているほうにx加速度を加える
    private void GenerateAttackVelocity()
    {
        attackVelocityTimer = player.attackVelocityDuration;
        player.SetVelocity(player.attackVelocity.x * player.facingDir, player.attackVelocity.y);
    }

}
