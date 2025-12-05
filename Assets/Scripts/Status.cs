using System;
using UnityEngine;

// ステータスの結果などを計算する責務
// ベース 10, 装備で +5　とかになった場合、その値の計算の責務
[Serializable]
public class Status
{
    [SerializeField] private float baseValue;
    [SerializeField] private float additiveBonus = 0; // 装備したらステータスに追加するなど
    [SerializeField] private float multiplier = 1f; // 何かのスキル起動時の倍率など

    public float GetValue()
    {
        return (baseValue + additiveBonus) * multiplier;
    }

    public void AddBonus (float v)
    {
        additiveBonus += v;
    }

    public void AddMultiplier(float v)
    {
        multiplier += v;
    }

    public void SetBaseValue (float v)
    {
        baseValue = v;
    }

}
