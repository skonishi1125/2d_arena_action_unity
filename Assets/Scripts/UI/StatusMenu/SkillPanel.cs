using UnityEngine;

public class SkillPanel : MonoBehaviour
{
    private EntityStatus status;
    private PlayerLevel level;

    public void Init(EntityStatus status, PlayerLevel level)
    {
        this.status = status;
        this.level = level;

        // skillのUIをなにか更新が必要な設計になった場合は、ここで改修
        // スキルレベルが上がったときなどに、イベントで紐づければ良さそう。

    }
}
