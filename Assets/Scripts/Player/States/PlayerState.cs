// Stateが、Playerのrb, anim等にアクセスするための抽象クラス
// EntityStateで用意すると、敵にEntityStateを使うときに邪魔になる
// ただし敵にもrb, animはあるため、変数定義だけはEntityStateで済ませておく
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


        // Dash
        if (input.Player.Dash.WasPressedThisFrame()
            && CanDashMovementConditions()
            && player.Skill.CanUse(SkillId.Dash))
        {
            player.Skill.OnUse(SkillId.Dash);
            stateMachine.ChangeState(player.dashState);
            return;
        }

        // teleport
        if (input.Player.Teleport.WasPerformedThisFrame())
        {
            stateMachine.ChangeState(player.teleportState);
            return;
        }

        // KnockbackAttack
        if (input.Player.KnockbackAttack.WasPerformedThisFrame()
            && player.Skill.CanUse(SkillId.HeavyKB))
        {
            player.Skill.OnUse(SkillId.HeavyKB);
            stateMachine.ChangeState(player.knockbackAttackState);
            return;
        }

        // MagicBolt
        if (input.Player.MagicBolt.WasPerformedThisFrame()
            && player.Skill.CanUse(SkillId.MagicBolt)) // 取得可否の確認
        {
            player.Skill.OnUse(SkillId.MagicBolt); // クールタイム処理
            stateMachine.ChangeState(player.magicBoltState);
            return;
        }



    }



    // ダッシュ条件(クールタイム以外)
    // 壁が目の前, ダッシュ中はダッシュできなくする
    private bool CanDashMovementConditions()
    {
        if (player.wallDetected)
            return false;

        if (stateMachine.currentState == player.dashState)
            return false;

        return true;
    }

    protected bool CanMultiJump()
    {
        if (input.Player.Jump.WasPerformedThisFrame() && player.jumpCount < player.maxJumps)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


}
