using UnityEngine;

public class VayuVFX : MonoBehaviour
{
    public ParticleSystem windLoopPS;
    public MuzzleFlash muzzleFlash;
    public string hitKey = "hit_vayu";

    void Start()
    {
        if (windLoopPS != null)
            windLoopPS.Play();
    }

    public void PlayShoot()
    {
        muzzleFlash?.Flash();
    }

    public void PlayHit(Vector3 position)
    {
        if (VFXManager.I != null)
            VFXManager.I.Play(hitKey, position);
    }
}
