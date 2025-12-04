using UnityEngine;

public class EntityCombat : MonoBehaviour
{


    // 攻撃モーション時のトリガー検知に関する情報
    [Header("Target detection")]
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius;
    [SerializeField] private LayerMask whatIsTarget;

    public void PerformAttack()
    {
        foreach (Collider2D targetCollider in GetDetectedColliders())
        {
            Debug.Log(targetCollider.name);
        }
    }

    // 攻撃判定内にいたcollider全てを配列で返す。
    private Collider2D[] GetDetectedColliders()
    {
        return Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatIsTarget);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }

}
