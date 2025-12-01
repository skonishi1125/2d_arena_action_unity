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
    public virtual void Update()
    {
    }

    // 出口処理 状態が終わったとき、次のStateを呼ぶときにやること
    public virtual void Exit()
    {
    }

}
