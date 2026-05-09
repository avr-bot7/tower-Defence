using System.Collections;
using UnityEngine;

public class VimanaEnemy : MonoBehaviour
{
    [Header("Flight")]
    public float flyHeight = 4f;
    public float bombCooldown = 3f;
    public float bombDamage = 20f;
    public float bombRadius = 2f;

    [Header("VFX")]
    public GameObject bombVFXPrefab;
    public GameObject engineTrailPrefab;

    float _groundY = 0f;

    [Tooltip("How far above the spawn point to start the ground-detection raycast.")]
    public float groundRayOriginHeight = 10f;
    [Tooltip("Maximum distance the ground-detection raycast travels downward.")]
    public float groundRayMaxDistance  = 20f;

    void Start()
    {
        // Detect the actual ground height at the spawn position so fly height
        // is relative to the Blender map surface, not the absolute world origin.
        if (Physics.Raycast(transform.position + Vector3.up * groundRayOriginHeight, Vector3.down, out RaycastHit hit, groundRayMaxDistance))
            _groundY = hit.point.y;
        else
            _groundY = transform.position.y; // fallback: stay at current height

        Vector3 pos = transform.position;
        pos.y = _groundY + flyHeight;
        transform.position = pos;

        if (engineTrailPrefab)
            Instantiate(engineTrailPrefab, transform);

        StartCoroutine(BombLoop());
    }

    void LateUpdate()
    {
        // Keep the Vimana pinned at groundY + flyHeight every frame.
        Vector3 p = transform.position;
        p.y = _groundY + flyHeight;
        transform.position = p;
    }

    IEnumerator BombLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(bombCooldown);
            DropBomb();
        }
    }

    void DropBomb()
    {
        var hits = Physics.OverlapSphere(new Vector3(transform.position.x, _groundY, transform.position.z), bombRadius);

        foreach (var hit in hits)
        {
            var tower = hit.GetComponent<TowerBase>();
            if (tower != null)
                StartCoroutine(StunTower(tower));
        }

        if (bombVFXPrefab)
            Instantiate(bombVFXPrefab, new Vector3(transform.position.x, _groundY + 0.5f, transform.position.z), Quaternion.identity);
    }

    IEnumerator StunTower(TowerBase tower)
    {
        float originalFireRate = tower.currentFireRate;
        tower.currentFireRate = 0f;

        yield return new WaitForSeconds(1.5f);

        if (tower != null)
            tower.currentFireRate = originalFireRate;
    }
}
