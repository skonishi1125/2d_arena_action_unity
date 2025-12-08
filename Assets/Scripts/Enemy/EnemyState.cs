using UnityEngine;

public class EnemyState : EntityState
{
    protected Enemy enemy;

    public EnemyState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.enemy = enemy;
        rb = enemy.rb;
        anim = enemy.anim;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        UpdateBaseAnimParams();
    }

    // 全ステート共通
    // 指定animパラメータの値にこちらを割り当てる
    // 速度が変更できる設定のanimatonは、数値に応じて早くなる
    protected virtual void UpdateBaseAnimParams()
    {
        anim.SetFloat("moveAnimSpeedMultiplier", enemy.moveAnimSpeedMultiplier);
        anim.SetFloat("xVelocity", rb.linearVelocity.x);
        // 敵のBattleState時のアニメの速さを、battleMoveSpeedに即した値にする
        float battleAnimSpeedMultiplier = enemy.battleMoveSpeed / enemy.moveSpeed;
        anim.SetFloat("battleAnimSpeedMultiplier", battleAnimSpeedMultiplier);
    }


}
