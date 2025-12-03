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

        if (Input.GetKeyDown(KeyCode.E))
            stateMachine.ChangeState(enemy.attackState);

        // 全ての敵stateで、指定animパラメータの値にこちらを割り当てる
        // 速度が変更できる設定のanimatonは、数値に応じて早くなる
        anim.SetFloat("moveAnimSpeedMultiplier", enemy.moveAnimSpeedMultiplier);
        anim.SetFloat("xVelocity", rb.linearVelocity.x);
    }


}
