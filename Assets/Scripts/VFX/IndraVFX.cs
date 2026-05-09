using UnityEngine;

public class IndraVFX : MonoBehaviour
{
    public ParticleSystem chargePS;
    public MuzzleFlash muzzleFlash;
    public string boltKey = "lightning_strike";

    public void PlayCharge()
    {
        if (chargePS == null) return;

        chargePS.Stop();
        chargePS.Play();
    }

    public void PlayBolt(Vector3 from, Vector3 to)
    {
        muzzleFlash?.Flash();
        DrawArcVFX(from, to);
    }

    void DrawArcVFX(Vector3 from, Vector3 to)
    {
        if (VFXManager.I == null) return;

        Vector3 direction = to - from;
        Quaternion rotation = direction.sqrMagnitude > 0.0001f
            ? Quaternion.LookRotation(direction)
            : Quaternion.identity;

        VFXManager.I.Play(boltKey, from, rotation);
    }
}
