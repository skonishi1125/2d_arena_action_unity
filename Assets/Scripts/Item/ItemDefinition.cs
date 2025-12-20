using UnityEngine;

public enum ItemEffectKind
{
    HealHp,
    TimedStatusDelta,
}

[CreateAssetMenu(menuName = "Game/Item Definition", fileName = "ItemDefinition_")]
public class ItemDefinition : ScriptableObject
{
    [Header("Effect")]
    public ItemEffectKind kind;

    [Tooltip("HealHp のとき使用")]
    public float healAmount = 20f;

    [Tooltip("TimedStatusDelta のとき使用")]
    public StatusParam statusParam = StatusParam.Attack;
    public ModifyMode modifyMode = ModifyMode.AddMultiplier;
    public float delta = 0.3f;     // 例：+0.3（= multiplier +0.3）
    public float duration = 5f;

    public void Apply(GameObject picker)
    {
        if (picker == null)
            return;

        switch (kind)
        {
            case ItemEffectKind.HealHp:
                {
                    // 現状、Itemを取るのはPlayer想定だけ。
                    var health = picker.GetComponentInParent<PlayerHealth>();
                    if (health != null)
                    {
                        health.Heal(healAmount);
                    }
                    break;
                }

            case ItemEffectKind.TimedStatusDelta:
                {
                    var timed = picker.GetComponentInParent<PlayerTimedModifiers>();
                    if (timed != null)
                    {
                        timed.ApplyTimed(statusParam, modifyMode, delta, duration);
                    }
                    break;
                }
        }
    }


}
