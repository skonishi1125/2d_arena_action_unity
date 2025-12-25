using UnityEngine;

public class EnemyMoveState : EnemyGroundState
{
    private int wallJumpAttempts;
    private float lastWallJumpTime;
    private bool isWallJumping;

    private float wallJumpStartTime;
    private float GroundIgnoreAfterWallJump = 0.10f;

    public EnemyMoveState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // idleから再び動き出すとき、反転
        if (!enemy.groundDetected || enemy.wallDetected)
            enemy.Flip();

        // 壁のジャンプ処理の初期化
        wallJumpAttempts = 0;
        isWallJumping = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // すでに壁ジャンプ中
        if (isWallJumping)
        {
            // 開始直後は ground 判定を無視
            if (Time.time < wallJumpStartTime + GroundIgnoreAfterWallJump)
                return;

            // 着地したら壁ジャンプ終了（後述の判定も推奨）
            if (enemy.groundDetected)
                isWallJumping = false;

            // ジャンプ中は他の判定をしない（壁も無視）
            return;
        }

        // 通常移動中：足場が消えたら idle へ（落下扱い）
        if (!enemy.groundDetected)
        {
            //Debug.Log("moveState: groundDetectedでidle");
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        // 地上かつ壁検知 → 壁ジャンプ or 方向転換
        if (enemy.wallDetected)
        {
            HandleWall();
            return;
        }

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (isWallJumping)
        {
            // 壁ジャンプ中は X 速度を維持
            float dir = enemy.facingDir;
            rb.linearVelocity = new Vector2(
                enemy.wallJumpVelocity.x * dir,
                rb.linearVelocity.y   // Yは物理挙動に任せる
            );
        }
        else
        {
            // 通常移動
            enemy.SetVelocity(enemy.moveSpeed * enemy.facingDir, rb.linearVelocity.y);
        }

    }

    // 壁にぶつかったときの挙動：
    // - 指定回数まではジャンプで乗り越えようとする
    // - それでもダメなら idle に戻して向きを変える
    private void HandleWall()
    {
        if (Time.time < lastWallJumpTime + enemy.wallJumpCooldown)
            return;

        // 規定回数だけジャンプを試す
        if (wallJumpAttempts < enemy.maxWallJumpAttempts)
        {
            wallJumpAttempts++;
            lastWallJumpTime = Time.time;
            isWallJumping = true;
            wallJumpStartTime = Time.time;

            float dir = enemy.facingDir;
            rb.linearVelocity = new Vector2(
                enemy.wallJumpVelocity.x * dir,
                enemy.wallJumpVelocity.y
            );

            return;
        }

        // ここまで来たら「何度か試したけど無理」→ 向きを変えてやり直す
        wallJumpAttempts = 0;
        isWallJumping = false;

        enemy.Flip(); // 反対方向へ歩き出す
        //Debug.Log("change!");
        stateMachine.ChangeState(enemy.idleState);
    }


}
