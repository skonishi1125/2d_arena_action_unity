using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class DemonHoundAnimationTrigger : EntityAnimationTrigger
{
    private DemonHound demonHound;

    protected override void Awake()
    {
        demonHound = GetComponentInParent<DemonHound>();
    }

    protected override void CurrentStateTrigger()
    {
        demonHound.CallAnimationTrigger();
    }

    // 突進 demonHound自体の速度を弄っているので責務に懸念がああるが、
    // 理解しやすい設計なので、今回はこのまま進める
    private void AttackVerocityTringger()
    {
        demonHound.SetVelocity(demonHound.chargeAttackXVelocity * demonHound.facingDir, 0f);
    }
}
