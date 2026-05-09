using UnityEngine;

public class VishnuVFX : MonoBehaviour
{
    public ParticleSystem idleGlowPS;
    public TrailRenderer chakraTrail;
    public string returnBurstKey = "chakra_return";

    void Start()
    {
        if (idleGlowPS != null)
            idleGlowPS.Play();
    }

    public void PlayReturnBurst()
    {
        if (VFXManager.I != null)
            VFXManager.I.Play(returnBurstKey, transform.position);
    }

    public static void AddTrailToDisc(GameObject disc)
    {
        if (disc == null) return;

        var trail = disc.AddComponent<TrailRenderer>();
        trail.time = 0.15f;
        trail.startWidth = 0.2f;
        trail.endWidth = 0f;
        trail.material = new Material(Shader.Find("Sprites/Default"));
        trail.startColor = new Color(1f, 0.85f, 0.1f, 1f);
        trail.endColor = new Color(1f, 0.5f, 0f, 0f);
    }
}
