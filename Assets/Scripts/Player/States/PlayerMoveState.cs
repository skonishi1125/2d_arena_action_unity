public class PlayerMoveState : PlayerGroundState
{
    public PlayerMoveState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    // 入力を受け付け、移動できるようにする
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // base内でステートが変わっていたら、以降の判定で上書きしない
        // この処理が無いと、停止と攻撃が同フレームで発生したとき、不具合が起こる
        // 停止とスキルが同時に行われたときに、クールタイムだけ適用されてスキルが出ないことがあった
        // これを書いておくと、別ステートになったときは、このMoveの以降の処理は走らなくなる
        // なので、xInputが0になったときなどでも処理を走らせないようにできる
        if (stateMachine.currentState != this)
            return;

        if (player.moveInput.x == 0 || player.wallDetected)
        {
            stateMachine.ChangeState(player.idleState, "Move: 移動入力がなくなった");
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        player.SetVelocity(player.moveInput.x * player.moveSpeed, rb.linearVelocity.y);
    }

}
