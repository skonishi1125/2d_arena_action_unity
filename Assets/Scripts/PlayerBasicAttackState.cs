using UnityEngine;

public class PlayerBasicAttackState : PlayerState
{
    // アニメータなど、最初のcomboIndexの値
    private const int FirstComboIndex = 1;

    // 攻撃中、前に進める猶予時間
    private float attackVelocityTimer;
    private int comboIndex = 1; // 1のとき、basicAttack1が発生する
    private int comboLimit = 3; // 最大3連コンボ
    private float lastTimeAttacked; // 最後に攻撃した時間の保持

    public PlayerBasicAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        if (comboLimit != player.attackVelocities.Length)
            Debug.LogWarning("PlayerBasicAttackState: 攻撃の数とplayer.attackVelocitiesの設定値が一致していません。");
    }

    public override void Enter()
    {
        base.Enter();
        ResetComboIndexIfNeeded();

        anim.SetInteger("basicAttackIndex", comboIndex);
        ApplyAttackVelocity();
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

    public override void Exit()
    {
        base.Exit();
        comboIndex++;
        lastTimeAttacked = Time.time;
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
    private void ApplyAttackVelocity()
    {
        attackVelocityTimer = player.attackVelocityDuration;
        player.SetVelocity(player.attackVelocities[comboIndex - 1].x * player.facingDir, player.attackVelocities[comboIndex - 1].y);
    }

    // 3回コンボが終わった後、indexが4などの値になるのを防ぐ
    private void ResetComboIndexIfNeeded()
    {
        // コンボ保持時間が切れた時、indexを1に戻す
        // 歩き回ってから2段目の攻撃が出ることを防ぐ。
        if (Time.time > lastTimeAttacked + player.comboResetTime)
            comboIndex = FirstComboIndex;

        if (comboIndex > comboLimit)
            comboIndex = FirstComboIndex;
    }

}
