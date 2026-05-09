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

    void Start()
    {
        Vector3 pos = transform.position;
        pos.y += flyHeight;
        transform.position = pos;

        if (engineTrailPrefab)
            Instantiate(engineTrailPrefab, transform);

        StartCoroutine(BombLoop());
    }

    void LateUpdate()
    {
        Vector3 p = transform.position;
        p.y = flyHeight;
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
        var hits = Physics.OverlapSphere(new Vector3(transform.position.x, 0f, transform.position.z), bombRadius);

        foreach (var hit in hits)
        {
            var tower = hit.GetComponent<TowerBase>();
            if (tower != null)
                StartCoroutine(StunTower(tower));
        }

        if (bombVFXPrefab)
            Instantiate(bombVFXPrefab, new Vector3(transform.position.x, 0.5f, transform.position.z), Quaternion.identity);
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
