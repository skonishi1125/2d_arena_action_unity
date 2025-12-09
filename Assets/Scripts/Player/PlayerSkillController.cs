using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    // 0 = 未習得, 1 = ダッシュだけ, 2以上 = ダッシュ+攻撃
     public int DashSkillLevel { get; private set; } = 2;

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
}
