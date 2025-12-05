using UnityEngine;

// 各種ステータスの結果、どんな値になったかだけを管理する責務
// ベース 10, 装備で +5　とかになった場合、結果である 15 を各種scriptに渡す。
public class EntityStatus : MonoBehaviour
{
    public Status maxHp;
    public Status regenHp;
    public Status maxMp;
    public Status regenMp;
    public Status attack;
    public Status defense;
    public Status evasion;
    public Status critical;

    public float GetMaxHp()
    {
        return maxHp.GetValue();
    }

    public float GetRegenHp()
    {
        return regenHp.GetValue();
    }

    public float GetMaxMp()
    {
        return maxMp.GetValue();
    }
    public float GetRegenMp()
    {
        return regenMp.GetValue();
    }

    public float GetAttack()
    {
        return attack.GetValue();
    }

    public float GetDefense()
    {
        return defense.GetValue();
    }

    public float GetEvasion()
    {
        return evasion.GetValue();
    }

    public float GetCritical()
    {
        return critical.GetValue();
    }



}
