using UnityEngine;

public class AgniVFX : MonoBehaviour
{
    public ParticleSystem idleFireLoop;
    public MuzzleFlash muzzleFlash;
    public string explosionKey = "explosion_agni";

    void Start()
    {
        if (idleFireLoop != null)
            idleFireLoop.Play();
    }

    public void PlayExplosion(Vector3 position)
    {
        if (VFXManager.I != null)
            VFXManager.I.Play(explosionKey, position);

        muzzleFlash?.Flash();
    }
}
