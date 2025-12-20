using UnityEngine;

public class Objective : MonoBehaviour
{
    private Rigidbody2D rb;

    public Animator anim { get; private set; }
    public EntityVFX entityVfx { get; private set; }
    public ObjectiveHealth Health { get; private set; }


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!LogHelper.AssertNotNull(rb, nameof(rb), this))
            return;

        anim = GetComponentInChildren<Animator>();
        if (!LogHelper.AssertNotNull(anim, nameof(anim), this))
            return;

        Health = GetComponent<ObjectiveHealth>();
        if (!LogHelper.AssertNotNull(Health, nameof(Health), this))
            return;

        entityVfx = GetComponent<EntityVFX>();
        if (!LogHelper.AssertNotNull(entityVfx, nameof(entityVfx), this))
            return;
    }



}
