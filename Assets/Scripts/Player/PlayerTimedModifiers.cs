using System;
using System.Collections;
using UnityEngine;

public class PlayerTimedModifiers : MonoBehaviour
{
    [SerializeField] private EntityStatus entityStatus;

    // ステータス変化時のイベント
    public event Action OnStatusChangedByItem;

    public void ApplyTimed(StatusParam param, ModifyMode mode, float delta, float duration)
    {
        var status = GetStatusRef(param);
        if (status == null) return;

        // 適用し、Status UI更新
        Apply(status, mode, delta);
        OnStatusChangedByItem?.Invoke();

        // 2) 期限が来たら戻す
        StartCoroutine(RemoveLater(status, mode, delta, duration));



        // VFXを出す

    }

    private IEnumerator RemoveLater(Status status, ModifyMode mode, float delta, float duration)
    {
        yield return new WaitForSeconds(duration);
        Apply(status, mode, -delta);
        OnStatusChangedByItem?.Invoke();
    }

    private void Apply(Status status, ModifyMode mode, float delta)
    {
        if (mode == ModifyMode.AddBonus) status.AddBonus(delta);
        else status.AddMultiplier(delta);
    }

    private Status GetStatusRef(StatusParam param)
    {
        if (entityStatus == null) return null;

        return param switch
        {
            StatusParam.Attack => entityStatus.attack,
            StatusParam.Defense => entityStatus.defense,
            StatusParam.MaxHp => entityStatus.maxHp,
            //StatusParam.RegenHp => entityStatus.regenHp,
            //StatusParam.MaxMp => entityStatus.maxMp,
            //StatusParam.RegenMp => entityStatus.regenMp,
            StatusParam.Evasion => entityStatus.evasion,
            StatusParam.Critical => entityStatus.critical,
            _ => null
        };
    }
}
