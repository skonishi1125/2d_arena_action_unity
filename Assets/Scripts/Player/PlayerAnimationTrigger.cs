using UnityEngine;

public class PlayerAnimationTrigger : EntityAnimationTrigger
{
    private Player player;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponentInParent<Player>();
        if (!LogHelper.AssertNotNull(player, nameof(player), this))
            return;

    }

    private void DashStartAttackTrigger()
    {
        if (player.Skill.DashHasAttack())
            entityCombat.PerformAttack();
    }

    private void DashEndAttackTrigger()
    {
        if (player.Skill.DashEndHasAttack())
            entityCombat.PerformAttack();
    }
}

