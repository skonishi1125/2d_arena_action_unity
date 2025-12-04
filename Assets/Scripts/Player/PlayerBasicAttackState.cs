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
    //private bool comboAttackQueued; // AttackState中に攻撃ボタンが連打されたとき、trueとなる
    // lastTimeAttackedで管理することにしたので使わなくなったが、実装が参考になるので残しておく
    private int attackDir; // 攻撃方向

    public PlayerBasicAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        if (comboLimit != player.attackVelocities.Length)
            Debug.LogWarning("PlayerBasicAttackState: 攻撃の数とplayer.attackVelocitiesの設定値が一致していません。");
    }

    public override void Enter()
    {
        base.Enter();
        //comboAttackQueued = false;
        ResetComboIndexIfNeeded();

        // 左右入力を受付け、攻撃方向を切り替えられるようにする
        attackDir = player.moveInput.x != 0 ? ((int)player.moveInput.x) : player.facingDir;

        anim.SetInteger("basicAttackIndex", comboIndex);
        ApplyAttackVelocity();
    }



    public override void LogicUpdate()
    {
        base.LogicUpdate();


        // 攻撃中に攻撃ボタンが押されたかどうかの判定
        if (input.Player.Attack.WasPressedThisFrame())
            player.lastAttackInputTime = Time.time;
        //comboAttackQueued = true;

        // triggerCalled:
        // 攻撃モーションの終わりにアニメーショントリガーとして以下を挟む。
        // EntityState.CallAnimationTrigger()
        // trueとなった場合、攻撃stateから抜け出し、idleに戻る。
        // idleに戻ったとき(Enter時)にはfalseとなり,再びこの値が使えるようになる。
        if (triggerCalled)
            HandleAttackStateExit();
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
        // プレイヤーが左右入力していたら、攻撃に横方向の加速度を加える
        if (player.moveInput.x != 0)
            player.SetVelocity(player.attackVelocities[comboIndex - 1].x * attackDir, player.attackVelocities[comboIndex - 1].y);
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

    // AttackStateから抜け出すときに考慮することをまとめる
    // * 攻撃が連打され、先行入力時間内
    //   -> AttackStateにもう一度遷移する。その時、Coroutineで 1F 待機して遷移する。
    // * 何も入力がない場合
    //   -> IdleStateへ。
    private void HandleAttackStateExit()
    {
        bool buffered =
            Time.time - player.lastAttackInputTime <= player.AttackInputBufferTime;

        // 攻撃ボタンが先行入力されていたら、続けて攻撃する
        // まだコンボ回数に余裕があり、先行入力されていたら次段へ
        if (buffered && comboIndex < comboLimit)
        {
            anim.SetBool(animBoolName, false); // 保険 やりたいのは、animBoolNameをfalseから、1F後にtrueにすること
            player.EnterAttackStateWithDelay();
        }
        else
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

}
