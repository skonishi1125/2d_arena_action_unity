using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    // 0 = 未習得,
    // 1 = ダッシュだけ,
    // 2 = ダッシュ開始に攻撃
    // 3 = ダッシュ終了時にも攻撃
     public int DashSkillLevel { get; private set; } = 0;
    public int MaxDashSkillLevel => 3;

    public void SetDashLevel(int level)
    {
        DashSkillLevel = Mathf.Max(0, level);
    }

    public bool CanUseDash()
    {
        return DashSkillLevel >= 1;
    }

    public bool DashHasAttack()
    {
        return DashSkillLevel >= 2;
    }

    public bool DashEndHasAttack()
    {
        return DashSkillLevel >= 3;
    }

    public void LevelUpDash()
    {
        if (DashSkillLevel >= MaxDashSkillLevel)
            return;

        DashSkillLevel++;
        Debug.Log($"Dash Skill Level Up: {DashSkillLevel}");
    }

}
