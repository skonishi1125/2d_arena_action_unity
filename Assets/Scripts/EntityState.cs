using UnityEngine;

public abstract class EntityState
{
    // 子クラス全てで使用するため, protected
    protected StateMachine stateMachine;
    protected string statename;

    // 敵味方共通のコンポーネントなど
    protected Rigidbody2D rb;
    // anim, collider, spriteなど必要になれば


    public EntityState(StateMachine stateMachine, string statename)
    {
        this.stateMachine = stateMachine;
        this.statename = statename;
    }

    // 入口処理 状態に入ったときにやること
    public virtual void Enter()
    {
    }

    // 状態中にやること
    // 論理演算系のUpdate
    public virtual void LogicUpdate()
    {
    }

    // 物理演算系(移動、ジャンプ)のUpdate()
    public virtual void PhysicsUpdate()
    {
    }

    // 出口処理 状態が終わったとき、次のStateを呼ぶときにやること
    public virtual void Exit()
    {
    }

}
