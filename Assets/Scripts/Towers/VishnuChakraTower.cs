using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VishnuChakraTower : MonoBehaviour
{
    [Header("Chakra Settings")]
    public float chakraSpeed = 14f;
    public float pierceDamage = 30f;
    public int maxPierces = 5;
    public float returnDelay = 0.3f;

    [Header("VFX")]
    public GameObject chakraPrefab;

    TowerBase _base;
    float _cd;
    bool _chakraOut;

    void Start() => _base = GetComponent<TowerBase>();

    void Update()
    {
        _cd -= Time.deltaTime;
        if (_cd > 0f || _chakraOut) return;

        Enemy target = _base.GetTargetPublic();
        if (target == null) return;

        StartCoroutine(LaunchChakra(target));
        _cd = 1f / _base.currentFireRate;
    }

    IEnumerator LaunchChakra(Enemy firstTarget)
    {
        _chakraOut = true;

        GameObject disc = chakraPrefab
            ? Instantiate(chakraPrefab, _base.shootPoint.position, Quaternion.identity)
            : CreateDefaultDisc();

        VishnuVFX.AddTrailToDisc(disc);

        var hit = new HashSet<Enemy>();
        int pierces = 0;

        Enemy current = firstTarget;
        while (current != null && pierces < maxPierces)
        {
            yield return StartCoroutine(MoveDisc(disc, () => current != null ? current.transform.position : disc.transform.position, 0.4f));
            if (current == null) break;

            if (!hit.Contains(current))
            {
                current.TakeDamage(_base.currentDamage + pierceDamage);
                hit.Add(current);
                pierces++;
            }

            yield return new WaitForSeconds(returnDelay);
            current = FindNextTarget(disc.transform.position, hit);
        }

        if (disc)
            yield return StartCoroutine(MoveDisc(disc, () => _base.shootPoint.position, 0.4f));

        GetComponent<VishnuVFX>()?.PlayReturnBurst();

        if (disc) Destroy(disc);
        _chakraOut = false;
    }

    IEnumerator MoveDisc(GameObject disc, Func<Vector3> targetPos, float arrivalDist)
    {
        while (disc != null)
        {
            Vector3 destination = targetPos();
            disc.transform.position = Vector3.MoveTowards(disc.transform.position, destination, chakraSpeed * Time.deltaTime);
            disc.transform.Rotate(Vector3.up, 400f * Time.deltaTime);

            if (Vector3.Distance(disc.transform.position, destination) < arrivalDist) yield break;
            yield return null;
        }
    }

    Enemy FindNextTarget(Vector3 from, HashSet<Enemy> exclude)
    {
        Enemy best = null;
        float bestDist = float.MaxValue;

        foreach (var enemy in FindObjectsOfType<Enemy>())
        {
            if (exclude.Contains(enemy)) continue;

            float d = Vector3.Distance(from, enemy.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = enemy;
            }
        }

        return best;
    }

    GameObject CreateDefaultDisc()
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.transform.localScale = new Vector3(0.6f, 0.05f, 0.6f);
        go.GetComponent<Renderer>().material.color = Color.yellow;
        Destroy(go.GetComponent<Collider>());
        return go;
    }
}
