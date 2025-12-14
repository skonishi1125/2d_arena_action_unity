using UnityEngine;

public class PlayerVFX : EntityVFX
{
    // テレポート
    [Header("Teleport VFX")]
    [SerializeField] private GameObject teleportVfx;
    [SerializeField] private Color teleportColor;

    [Header("Level Up VFX")]
    [SerializeField] private GameObject levelUpVfx;

    public void CreateOnTeleportVfx(Transform target)
    {
        GameObject vfx = Instantiate(teleportVfx, target.position, Quaternion.identity);
        vfx.GetComponentInChildren<SpriteRenderer>().color = teleportColor;

    }

    public void CreateOnLevelUpVfx(Transform target)
    {
        GameObject vfx = Instantiate(levelUpVfx, target.position, Quaternion.identity);
    }


}
