using UnityEngine;

public class EnemyBattleState : EnemyState
{
    private Transform player;
    private float verticalOutOfRangeTimer; // 縦に離れた時間のタイマー

    // ジャンプ関連
    private int wallJumpAttempts;
    private float lastWallJumpTime;
    private bool isWallJumping;

    public EnemyBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // 感知から状態に入ったケース, 後ろからplayerに殴られたケースなどを考慮
        player ??= enemy.GetPlayerReference(); // if(player == null) player = ...と同じ

        wallJumpAttempts = 0;
        isWallJumping = false;

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // ① 壁ジャンプ中の処理
        if (isWallJumping)
        {
            if (enemy.groundDetected)
                isWallJumping = false;

            // ジャンプ中は攻撃開始判定や縦距離チェックをしない
            return;
        }

        // 1. 縦方向のチェック
        HandleVerticalRange();

        // 2. 攻撃開始判定（縦が許容範囲内のときだけ）
        if (IsVerticallyInRange())
            TryStartAttack();
    }

    // virtualとして、DarkKnightなどでオーバーライドできるようにする
    protected virtual void TryStartAttack()
    {
        if (WithinAttackRange())
        {
            FaceToPlayer();

            var nextAttack = enemy.GetNextAttackState();
            if (nextAttack != null)
                stateMachine.ChangeState(nextAttack);
        }
    }


    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // ① 壁ジャンプ中の移動
        if (isWallJumping)
        {
            float dir = enemy.facingDir;
            rb.linearVelocity = new Vector2(
                enemy.wallJumpVelocity.x * dir,
                rb.linearVelocity.y   // Yは重力に任せる
            );
            return;
        }

        // ② 攻撃範囲外ならプレイヤーに近づく
        if (!WithinAttackRange())
        {
            // 壁に当たっていて、かつ地上にいるならジャンプを試す
            if (enemy.wallDetected && enemy.groundDetected)
            {
                HandleWallInBattle();
            }
            else
            {
                Debug.Log("setvelocity?");

                enemy.SetVelocity(
                    enemy.battleMoveSpeed * DirectionToPlayer(),
                    rb.linearVelocity.y
                );
            }
        }

        Debug.Log("battlestate update");
    }


    // Playerとの距離が、攻撃範囲より小さくなったらtrueを返す
    protected virtual bool WithinAttackRange()
    {
        return DistanceToPlayer() < enemy.attackDistance;
    }

    // 絶対値で、playerのx座標 - 敵のx座標の結果を返す
    protected float DistanceToPlayer()
    {
        // 感知できない場合は、遠い位置にいるため最大値を返しておく
        if (player == null)
            return float.MaxValue;

        return Mathf.Abs(player.position.x - enemy.transform.position.x);
    }

    // Player方向へ移動したいときの「移動ベクトル方向」を返す。
    // 見た目の向きを決めるためではなく、SetVelocity で使うための力学的方向判定。
    // （敵がプレイヤーに接近したいときのみ使用する）
    private int DirectionToPlayer()
    {
        if (player == null)
            return 0;

        if (player.position.x > enemy.transform.position.x)
            return 1;
        else
            return -1;

    }

    // 見た目（Flip）の向きをプレイヤー方向へ揃える。
    // 攻撃直前など、「移動しなくても向きを合わせたい」場面で使用する。
    // 移動ベクトルとは独立している点に注意。
    protected void FaceToPlayer()
    {
        Transform p = enemy.GetPlayerReference();
        if (p == null)
            return;
        float dx = p.position.x - enemy.transform.position.x;
        // プレイヤーが右にいて、今左を向いているなら右向きに
        if (dx > 0 && enemy.facingDir < 0)
            enemy.Flip();
        // プレイヤーが左にいて、今右を向いているなら左向きに
        else if (dx < 0 && enemy.facingDir > 0)
            enemy.Flip();
    }

    // プレイヤーとの縦方向の差が、許容範囲内かどうか
    private bool IsVerticallyInRange()
    {
        if (player == null)
            return false;

        float dy = Mathf.Abs(player.position.y - enemy.transform.position.y);
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

        // ここまで来たら「何度か試したけどダメ」→ 方向転換して追尾を中止
        wallJumpAttempts = 0;
        isWallJumping = false;

        enemy.Flip();
        stateMachine.ChangeState(enemy.idleState);
    }

}
