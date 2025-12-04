using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class DemonHoundAnimationTrigger : EntityAnimationTrigger
{
    private DemonHound demonHound;

    protected override void Awake()
    {
        base.Awake();
        demonHound = GetComponentInParent<DemonHound>();
    }

    // 突進 demonHound自体の速度を弄っているので責務に懸念があるが、
    // 理解しやすい設計なので、今回はこのまま進める
    protected override void AttackTrigger()
    {
        base.AttackTrigger();
        demonHound.SetVelocity(demonHound.chargeAttackXVelocity * demonHound.facingDir, 0f);
    }
}
