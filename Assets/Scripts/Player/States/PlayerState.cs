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

        // Zスキル
        if (input.Player.SkillZ.WasPressedThisFrame())
        {
            var id = player.Skill.GetEquipped(SkillSlot.Z);

            if (id == SkillId.Dash && CanDashMovementConditions() && player.Skill.CanUse(id))
            {
                player.Skill.OnUse(id);
                stateMachine.ChangeState(player.dashState);
                return;
            }

            if (id == SkillId.Teleport && player.Skill.CanUse(id))
            {
                player.Skill.OnUse(id);
                stateMachine.ChangeState(player.teleportState);
                return;
            }
        }

        // Dスキル 現状 KnockbackAttack だけ
        if (input.Player.KnockbackAttack.WasPerformedThisFrame()
            && player.Skill.CanUse(SkillId.HeavyKB))
        {
            player.Skill.OnUse(SkillId.HeavyKB);
            stateMachine.ChangeState(player.knockbackAttackState);
            return;
        }

        // Vスキル
        if (input.Player.SkillV.WasPressedThisFrame())
        {
            var id = player.Skill.GetEquipped(SkillSlot.V);

            if (id == SkillId.GroundSlam && player.Skill.CanUse(id))
            {
                player.Skill.OnUse(id);
                stateMachine.ChangeState(player.groundSlamJumpState);
                return;
            }

            if (id == SkillId.MagicBolt && player.Skill.CanUse(id))
            {
                player.Skill.OnUse(id);
                stateMachine.ChangeState(player.magicBoltState);
                return;
            }
        }

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
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
