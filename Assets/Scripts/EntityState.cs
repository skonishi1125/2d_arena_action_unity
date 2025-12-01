using UnityEngine;

public abstract class EntityState
{
    // 子クラス全てで使用するため, protected
    protected StateMachine stateMachine;
    protected string animBoolName;

    // 敵味方共通のコンポーネントなど
    protected Rigidbody2D rb;
    // anim, collider, spriteなど必要になれば


    public EntityState(StateMachine stateMachine, string animBoolName)
    {
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    // 入口処理 状態に入ったときにやること
    public virtual void Enter()
    {
    }

    // 状態中にやること
    // 論理演算系のUpdate。状態遷移、入力受付, anim切替, 条件式評価
    // フレームの最終決定
    public virtual void LogicUpdate()
    {
    }

    // 物理演算系(移動、ジャンプ)のUpdate()
    // rbに速度加算, addForce, 物理挙動などrg, coを調整
    public virtual void PhysicsUpdate()
    {
    }

    // 出口処理 状態が終わったとき、次のStateを呼ぶときにやること
    public virtual void Exit()
    {
    }

}
