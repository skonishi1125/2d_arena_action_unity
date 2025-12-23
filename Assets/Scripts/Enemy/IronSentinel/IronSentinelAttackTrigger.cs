using UnityEngine;

public class IronSentinelAttackTrigger : EntityAnimationTrigger
{
    private IronSentinel ironSentinel;

    protected override void Awake()
    {
        base.Awake();
        ironSentinel = GetComponentInParent<IronSentinel>();
    }

    // スタブ突進(突進のみ) 攻撃の最初のみ突進とAttackTriggerを同時に実行
    // AttackTriggerをフレームにつけて持続攻撃にする
    private void xVelocityMultiplier()
    {
        ironSentinel.SetVelocity(ironSentinel.dashAttackXVelocity * ironSentinel.facingDir, 0f);
    }

    private void xVelocityZero()
    {
        ironSentinel.SetVelocity(0f, 0f);
    }

}
