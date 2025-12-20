using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/PlayerLevelTable")]
public class PlayerLevelTable : ScriptableObject
{
    [Serializable]
    public class LevelEntry
    {
        public int level;
        public int requiredExp;
        public float maxHp;
        public float regenHp;
        public float maxMp;
        public float regenMp;
        public float attack;
        public float defense;
        public float evasion;
        public float critical;
    }

    public LevelEntry[] levels; // ScriptableObjectに登録されたデータ
    public int MaxLevel => levels.Length;

    // 指定したレベルの各ステータスを返す
    public LevelEntry GetLevelOfInfo(int level)
    {
        // ex) 1の時 levels[0] 3の時 levels[2] を返す
        int index = Mathf.Clamp(level - 1, 0, levels.Length - 1);
        return levels[index];
    }

}
