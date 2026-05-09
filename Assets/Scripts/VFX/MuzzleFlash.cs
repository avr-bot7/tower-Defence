using System.Collections;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    [Header("Flash Settings")]
    public float flashDuration = 0.08f;
    public Light flashLight;
    public float lightIntensity = 3f;

    ParticleSystem _particleSystem;

    void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        if (flashLight != null)
            flashLight.enabled = false;
    }

    public void Flash()
    {
        if (_particleSystem != null)
            _particleSystem.Play();

        if (flashLight != null)
            StartCoroutine(LightFlash());
    }

    IEnumerator LightFlash()
    {
        flashLight.enabled = true;
        flashLight.intensity = lightIntensity;

        yield return new WaitForSeconds(flashDuration);

        flashLight.enabled = false;
    }
}
