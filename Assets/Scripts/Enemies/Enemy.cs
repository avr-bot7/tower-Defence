// ============================================================
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public System.Func<float, float> OnTakeDamage;

    [Header("Stats")]
    public float maxHP    = 100f;
    public float speed    = 5f;
    public int   goldDrop = 15;
    public int   damage   = 1;   // lives lost when reaching end

    [Header("UI")]
    public Slider hpBar;

    [HideInInspector] public float hp;

    void Start()
    {
        hp = maxHP;
        if (hpBar) hpBar.value = 1f;
    }

    public void TakeDamage(float dmg)
    {
        if (OnTakeDamage != null)
            dmg = OnTakeDamage(dmg);

        hp -= dmg;
        if (hpBar) hpBar.value = hp / maxHP;
        if (hp <= 0) Die();
    }

    public void Die()
    {
        // 1. Trigger animation
        var anim = GetComponentInChildren<Animator>();
        if (anim) anim.SetTrigger("Die");

        // 2. Stop movement and disable collider
        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent) agent.isStopped = true;
        
        var col = GetComponent<Collider>();
        if (col) col.enabled = false;

        // 3. Delay destroy so death animation plays
        StartCoroutine(DestroyAfterAnim(anim));
    }

    System.Collections.IEnumerator DestroyAfterAnim(Animator anim)
    {
        if (anim != null)
        {
            // Wait for death animation length + a small buffer
            yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length + 0.1f);
        }
        else yield return new WaitForSeconds(1.5f);

        GameManager.I.AddGold(100);
        if (WaveManager.I != null)
            WaveManager.I.OnEnemyRemoved();

        Destroy(gameObject);
    }

    public void ReachEnd()
    {
        GameManager.I.LoseLife();
        if (WaveManager.I != null)
            WaveManager.I.OnEnemyRemoved();
        Destroy(gameObject);
    }
}
