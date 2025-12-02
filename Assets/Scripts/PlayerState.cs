// Stateが、Playerのrb, anim等にアクセスするための抽象クラス
// EntityStateで用意すると、敵にEntityStateを使うときに邪魔になる
// ただし敵にもrb, animはあるため、変数定義だけはEntityStateで済ませておく
using UnityEditor;

public abstract class PlayerState : EntityState
{
    protected Player player;
    protected PlayerInputSet input;

    protected PlayerState(Player player, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.player = player;
        rb = player.rb;
        anim = player.anim;
        input = player.input;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        // Jump/Fallアニメの切り替えはPlayerだけなので、EntityStateには書かない。
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }
}
