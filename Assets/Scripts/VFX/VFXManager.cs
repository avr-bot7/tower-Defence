using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager I;

    [System.Serializable]
    public class VFXEntry
    {
        public string key;
        public GameObject prefab;
        public int poolSize = 8;
    }

    public VFXEntry[] entries;

    readonly Dictionary<string, Queue<GameObject>> _pools = new();
    readonly Dictionary<string, GameObject> _prefabs = new();

    void Awake()
    {
        I = this;

        foreach (var entry in entries)
        {
            if (entry == null || string.IsNullOrWhiteSpace(entry.key) || entry.prefab == null) continue;

            var queue = new Queue<GameObject>();
            for (int i = 0; i < Mathf.Max(1, entry.poolSize); i++)
            {
                var go = Instantiate(entry.prefab, transform);
                go.SetActive(false);
                queue.Enqueue(go);
            }

            _pools[entry.key] = queue;
            _prefabs[entry.key] = entry.prefab;
        }
    }

    public void Play(string key, Vector3 position, Quaternion rotation = default)
    {
        if (!_pools.TryGetValue(key, out var pool) || !_prefabs.TryGetValue(key, out var prefab))
        {
            Debug.LogWarning($"VFX key '{key}' not found");
            return;
        }

        GameObject go = pool.Count > 0 ? pool.Dequeue() : Instantiate(prefab, transform);
        if (rotation == default)
            rotation = Quaternion.identity;

        go.transform.SetPositionAndRotation(position, rotation);
        go.SetActive(true);

        float duration = GetDuration(go);
        StartCoroutine(ReturnToPool(go, key, duration));
    }

    float GetDuration(GameObject go)
    {
        var ps = go.GetComponent<ParticleSystem>();
        if (ps == null) return 2f;

        var main = ps.main;
        return main.duration + main.startLifetime.constantMax;
    }

    IEnumerator ReturnToPool(GameObject go, string key, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (go == null) yield break;
        go.SetActive(false);

        if (_pools.TryGetValue(key, out var pool))
            pool.Enqueue(go);
    }
}
