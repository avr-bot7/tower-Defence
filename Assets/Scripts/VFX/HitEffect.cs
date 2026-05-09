using UnityEngine;

public class HitEffect : MonoBehaviour
{
    [Header("Per-Tower Type Keys")]
    public string vfxKey = "hit_default";

    public void Trigger(Vector3 position)
    {
        if (VFXManager.I != null)
            VFXManager.I.Play(vfxKey, position);
    }
}
