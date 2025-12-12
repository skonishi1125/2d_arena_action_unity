using UnityEngine;

public class PlayerVFX : EntityVFX
{
    // テレポート
    [Header("Teleport VFX")]
    [SerializeField] private GameObject teleportVfx;
    [SerializeField] private Color teleportColor;

    public void CreateOnTeleportVfx(Transform target)
    {
        GameObject vfx = Instantiate(teleportVfx, target.position, Quaternion.identity);
        vfx.GetComponentInChildren<SpriteRenderer>().color = teleportColor;

    }


}
