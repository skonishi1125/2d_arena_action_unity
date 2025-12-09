public class KnockbackSkillButton : SkillButton
{
    protected override SkillId TargetSkillId => SkillId.KnockbackAttack;
    protected override string GetLevelLabel(int level)
    {
        return $"KB Lv.{level}";
    }
}
