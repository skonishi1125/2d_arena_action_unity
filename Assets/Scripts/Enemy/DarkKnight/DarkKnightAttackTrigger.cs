using UnityEngine;

public class DarkKnightAttackTrigger : EntityAnimationTrigger
{
    private DarkKnight darkKnight;

    protected override void Awake()
    {
        base.Awake();
        darkKnight = GetComponentInParent<DarkKnight>();
    }

    // スタブ突進(突進のみ) 攻撃の最初のみ突進とAttackTriggerを同時に実行
    // AttackTriggerをフレームにつけて持続攻撃にする
    private void xVelocityMultiplier ()
    {
        darkKnight.SetVelocity(darkKnight.dashAttackXVelocity * darkKnight.facingDir, 0f);
    }

    private void xVelocityZero()
    {
        darkKnight.SetVelocity(0f, 0f);
    }

}
