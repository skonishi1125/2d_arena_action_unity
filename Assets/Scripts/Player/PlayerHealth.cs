using UnityEngine;

public class PlayerHealth : EntityHealth
{

    protected override void Die()
    {
        base.Die();

        // you died!みたいなUIを併せて出す
        GameManager.Instance.GameOver();
    }
}
