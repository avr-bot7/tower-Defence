using System.Collections;
using UnityEngine;

public class EnemyDeathVFX : MonoBehaviour
{
    public string deathVFXKey = "death_asura";
    public float deathDelay = 0.1f;

    public IEnumerator PlayAndDestroy()
    {
        if (VFXManager.I != null)
            VFXManager.I.Play(deathVFXKey, transform.position);

        foreach (var rendererComp in GetComponentsInChildren<Renderer>())
            rendererComp.enabled = false;

        foreach (var colliderComp in GetComponentsInChildren<Collider>())
            colliderComp.enabled = false;

        yield return new WaitForSeconds(deathDelay);
        Destroy(gameObject);
    }
}
