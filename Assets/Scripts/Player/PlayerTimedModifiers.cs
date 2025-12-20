using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTimedModifiers : MonoBehaviour
{
    [SerializeField] private EntityStatus entityStatus;

    [SerializeField] private PlayerVFX playerVfx;
    private readonly Dictionary<StatusParam, int> activeCounts = new();
    [Header("Buff Colors")]
    [SerializeField] private Color attackBuffColor = new Color(1f, 0.3f, 0.3f, 1f);
    [SerializeField] private Color defenseBuffColor = new Color(0.3f, 0.6f, 1f, 1f);
    [SerializeField] private Color critBuffColor = new Color(1f, 1f, 0.3f, 1f);

    // ステータス変化時のイベント
    public event Action OnStatusChangedByItem;

    private void Awake()
    {
        entityStatus = GetComponent<EntityStatus>();
        playerVfx = GetComponent<PlayerVFX>();
    }

    public void ApplyTimed(StatusParam param, ModifyMode mode, float delta, float duration)
    {
        var status = GetStatusRef(param);
        if (status == null)
            return;

        Apply(status, mode, delta);
        OnStatusChangedByItem?.Invoke();

        BeginBuffVfx(param);

        StartCoroutine(RemoveLater(status, mode, delta, duration, param));
    }

    private Color GetBuffColor(StatusParam param)
    {
        return param switch
        {
            StatusParam.Attack => attackBuffColor,
            StatusParam.Defense => defenseBuffColor,
            StatusParam.Critical => critBuffColor,
            _ => Color.white
        };
    }

    private void EndBuffVfx(StatusParam param)
    {
        if (playerVfx == null) return;

        if (!activeCounts.TryGetValue(param, out int count))
            return;

        count--;
        if (count <= 0)
        {
            activeCounts.Remove(param);
            playerVfx.StopBuffOverlay();
        }
        else
        {
            activeCounts[param] = count;
        }
    }

    private void BeginBuffVfx(StatusParam param)
    {
        if (playerVfx == null) return;

        activeCounts.TryGetValue(param, out int count);
        count++;
        activeCounts[param] = count;

        // 初回だけ開始
        if (count == 1)
            playerVfx.PlayBuffOverlay(GetBuffColor(param));
    }

    private IEnumerator RemoveLater(Status status, ModifyMode mode, float delta, float duration, StatusParam param)
    {
        yield return new WaitForSeconds(duration);

        Apply(status, mode, -delta);
        OnStatusChangedByItem?.Invoke();

        EndBuffVfx(param);
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
