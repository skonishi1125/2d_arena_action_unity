public class DashSkillButton : SkillButton
{
    protected override SkillId TargetSkillId => SkillId.Dash;

    protected override string GetLevelLabel(int level)
    {
        return $"Dash Lv.{level}";
    }
}
