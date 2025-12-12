using UnityEngine;

public class Chest : MonoBehaviour, IDamagable
{
    private EntityVFX entityVfx;

    private Animator anim;
    private Rigidbody2D rb;

    [SerializeField] private Vector2 openKnockback;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        entityVfx = GetComponent<EntityVFX>();

        anim = GetComponentInChildren<Animator>();
    }
    public void TakeDamage(DamageContext ctx)
    {
        anim.SetBool("chestOpen", true);
        rb.linearVelocity = openKnockback;
        rb.angularVelocity = Random.Range(-200, 200);
        entityVfx.PlayOnDamageVfx();
    }
}
