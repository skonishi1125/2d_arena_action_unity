using UnityEngine;

public class PlayerTeleportState : PlayerState
{
    private Vector2 targetPos;
    private bool teleported;

    public PlayerTeleportState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // 速度リセット（Dashと違い、物理で移動しない）
        rb.linearVelocity = Vector2.zero;

        // 入力方向（左右だけ見る）
        int dir = player.moveInput.x != 0 ? (int)Mathf.Sign(player.moveInput.x) : player.facingDir;

        Vector2 start = rb.position;
        Vector2 desired = start + Vector2.right * dir * player.teleportDistance;

        // 安全位置に補正（壁内・地面内に入らないように）
        targetPos = FindSafeTeleportPos(start, desired);

        // 瞬間移動
        rb.position = targetPos;
        teleported = true;

        // VFX
        player.Vfx.CreateOnTeleportVfx(player.transform);

        // ここでは遷移しない（判定更新が1フレーム遅れる可能性がある）
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!teleported) return;

        // 次のUpdateで衝突判定が更新された後に遷移する想定
        if (player.groundDetected)
            stateMachine.ChangeState(player.idleState);
        else
            stateMachine.ChangeState(player.fallState);
    }

    private Vector2 FindSafeTeleportPos(Vector2 start, Vector2 desired)
    {
        Vector2 dir = (desired - start).normalized;
        float dist = Vector2.Distance(start, desired);

        // 目的地から手前に向かって、当たり判定が埋まらない地点を探す
        for (float d = dist; d >= 0f; d -= player.teleportCheckStep)
        {
            Vector2 p = start + dir * d;

            // CapsuleCollider2Dの代わりに簡易的に円でチェック
            // whatIsGround で「壁や床」を塞ぐ前提
            bool blocked = Physics2D.OverlapCircle(p, player.teleportRadius, player.whatIsGround);
            if (!blocked)
                return p;
        }

        // どこも無理なら元の位置
        return start;
    }

    public override void Exit()
    {
        base.Exit();
    }

}
