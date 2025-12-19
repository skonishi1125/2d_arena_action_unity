using UnityEngine;

public class Objective : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;

    public ObjectiveHealth Health { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!LogHelper.AssertNotNull(rb, nameof(rb), this))
            return;

        anim = GetComponentInChildren<Animator>();

        Health = GetComponent<ObjectiveHealth>();
        if (!LogHelper.AssertNotNull(Health, nameof(Health), this))
            return;

    }



}
