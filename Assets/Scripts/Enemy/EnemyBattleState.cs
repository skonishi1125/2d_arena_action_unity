using UnityEngine;

public class EnemyBattleState : EnemyState
{
    protected Transform target;
    private float verticalOutOfRangeTimer; // 縦に離れた時間のタイマー

    // ジャンプ関連
    private int wallJumpAttempts;
    private float lastWallJumpTime;
    private bool isWallJumping;

    // Raider用 Playerを感知したら、その間Targetを更新
    private float nextSenseTime;
    private const float SenseInterval = 0.2f;

    public EnemyBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        nextSenseTime = Time.time;

        // 今狙うべき相手のTransformを取得
        // Playerなのか、Objectiveなのか。
        target = enemy.GetCurrentTarget();

        wallJumpAttempts = 0;
        isWallJumping = false;
        verticalOutOfRangeTimer = 0f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // RaiderがObjectiveを追っている間だけ、定期的にPlayer検知してAggroへ
        if (enemy.Role == EnemyRole.Raider && !enemy.IsAggroingPlayer && Time.time >= nextSenseTime)
        {
            nextSenseTime = Time.time + SenseInterval;

            var hit = enemy.PlayerDetection();
            if (hit)
                enemy.TryEnterBattleState(hit.transform); // battle中でもplayer更新＆aggro延長できる
        }

        // ターゲット更新（Aggro切替に追従）
        target = enemy.GetCurrentTarget();
        if (target == null)
        {
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        // 壁ジャンプ中の処理
        if (isWallJumping)
        {
            if (enemy.groundDetected)
                isWallJumping = false;

            // ジャンプ中は攻撃開始判定や縦距離チェックをしない
            return;
        }

        // ターゲット対象がPlayerなら、縦判定の見逃し処理を使う
        bool targetIsPlayer = enemy.IsTargetPlayer(target);
        if (targetIsPlayer)
        {
            HandleVerticalRange();
            if (IsVerticallyInRange())
                TryStartAttack();
        }
        else
        {
            TryStartAttack();
        }

    }

    // virtualとして、DarkKnightなどでオーバーライドできるようにする
    protected virtual void TryStartAttack()
    {
        if (WithinAttackRange())
        {
            FaceToTarget();

            var nextAttack = enemy.GetNextAttackState();
            if (nextAttack != null)
                stateMachine.ChangeState(nextAttack);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // ターゲット更新
        target = enemy.GetCurrentTarget();
        if (target == null)
            return;

        // 壁ジャンプ中の移動
        if (isWallJumping)
        {
            float dir = enemy.facingDir;
            rb.linearVelocity = new Vector2(
                enemy.wallJumpVelocity.x * dir,
                rb.linearVelocity.y   // Yは重力に任せる
            );
            return;
        }

        // 攻撃範囲外ならプレイヤーに近づく
        if (!WithinAttackRange())
        {
            // 壁に当たっていて、かつ地上にいるならジャンプを試す
            if (enemy.wallDetected && enemy.groundDetected)
            {
                HandleWallInBattle();
            }
            else
            {
                enemy.SetVelocity(
                    enemy.battleMoveSpeed * DirectionToTarget(),
                    rb.linearVelocity.y
                );
            }
        }
    }


    // Targetとの距離が、攻撃範囲より小さくなったらtrueを返す
    protected virtual bool WithinAttackRange()
    {
        return DistanceToTarget() < enemy.attackDistance;
    }

    // 絶対値で、targetのx座標 - 敵のx座標の結果を返す
    protected float DistanceToTarget()
    {
        // 感知できない場合は、遠い位置にいるため最大値を返しておく
        if (target == null)
            return float.MaxValue;

        return Mathf.Abs(target.position.x - enemy.transform.position.x);
    }

    // Target方向へ移動したいときの「移動ベクトル方向」を返す。
    // 見た目の向きを決めるためではなく、SetVelocity で使うための力学的方向判定。
    // （敵がプレイヤーに接近したいときのみ使用する）
    private int DirectionToTarget()
    {
        if (target == null)
            return 0;

        if (target.position.x > enemy.transform.position.x)
            return 1;
        else
            return -1;

    }

    // 現在の向いているほうをtarget方向にする
    protected void FaceToTarget()
    {

        if (target == null)
            return;
        float dx = target.position.x - enemy.transform.position.x;

        // targetが右にいて、今左を向いているなら右向きに
        if (dx > 0 && enemy.facingDir < 0)
            enemy.Flip();
        // targetが左にいて、今右を向いているなら左向きに
        else if (dx < 0 && enemy.facingDir > 0)
            enemy.Flip();
    }

    // targetとの縦方向の差が、許容範囲内かどうか
    // (※targetがPlayerの時しか走らせないように、呼び出し側で分岐している）
    private bool IsVerticallyInRange()
    {
        if (target == null)
            return false;

        float dy = Mathf.Abs(target.position.y - enemy.transform.position.y);
        return dy <= enemy.attackVerticalRange;
    }

    // 縦方向が一定時間以上離れていたら idleState へ戻す
    private void HandleVerticalRange()
    {
        if (!IsVerticallyInRange())
        {
            verticalOutOfRangeTimer += Time.deltaTime;

            if (verticalOutOfRangeTimer >= enemy.verticalOutOfRangeTime)
                stateMachine.ChangeState(enemy.idleState);
        }
        else
        {
            // 範囲内に戻ってきたらタイマーリセット
            verticalOutOfRangeTimer = 0f;
        }
    }

    // 壁ジャンプ処理
    private void HandleWallInBattle()
    {
        // クールタイム中は何もしない（諦めない）
        if (Time.time < lastWallJumpTime + enemy.wallJumpCooldown)
            return;

        // 指定回数だけジャンプを試す
        if (wallJumpAttempts < enemy.maxWallJumpAttempts)
        {
            wallJumpAttempts++;
            lastWallJumpTime = Time.time;
            isWallJumping = true;

            // いま向いている方向へジャンプ
            float dir = enemy.facingDir;

            // ジャンプ初速
            rb.linearVelocity = new Vector2(
                enemy.wallJumpVelocity.x * dir,
                enemy.wallJumpVelocity.y
            );

            return;
        }

        // 何度か試したけどダメだったら、 諦めて方向転換
        wallJumpAttempts = 0;
        isWallJumping = false;

        enemy.Flip();
        stateMachine.ChangeState(enemy.idleState);
    }

}
