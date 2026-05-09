using System.Collections;
using TMPro;
using UnityEngine;

public class MahishasuraBoss : MonoBehaviour
{
    [Header("Phases")]
    public float phase2Threshold = 0.60f;
    public float phase3Threshold = 0.30f;

    [Header("Phase 2 - Summon")]
    public GameObject minionPrefab;
    public int minionCount = 4;
    public float summonCooldown = 8f;

    [Header("Phase 2 - Shield")]
    public float shieldHP = 150f;
    public GameObject shieldVFXPrefab;

    [Header("Phase 3 - Berserk")]
    public float berserkSpeedMult = 2.2f;

    [Header("UI")]
    public GameObject bossHUDPrefab;
    public TextMeshProUGUI bossNameText;

    enum Phase
    {
        One,
        Two,
        Three
    }

    Phase _phase = Phase.One;
    float _shieldCurrent;
    bool _shielded;
    bool _slowImmune;

    GameObject _shieldVFX;
    Enemy _enemy;

    void Start()
    {
        _enemy = GetComponent<Enemy>();
        if (_enemy != null)
            _enemy.OnTakeDamage += InterceptDamage;

        if (bossHUDPrefab)
            Instantiate(bossHUDPrefab, transform);

        if (bossNameText)
            bossNameText.text = "Mahishasura";

        StartCoroutine(BossEntrance());
    }

    void OnDestroy()
    {
        if (_enemy != null)
            _enemy.OnTakeDamage -= InterceptDamage;
    }

    IEnumerator BossEntrance()
    {
        Time.timeScale = 0.4f;
        yield return new WaitForSecondsRealtime(1.2f);
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (_enemy == null || _enemy.maxHP <= 0f) return;

        float hpPercent = _enemy.hp / _enemy.maxHP;

        if (_phase == Phase.One && hpPercent <= phase2Threshold)
            EnterPhase2();
        else if (_phase == Phase.Two && hpPercent <= phase3Threshold)
            EnterPhase3();
    }

    void EnterPhase2()
    {
        _phase = Phase.Two;
        ActivateShield();
        StartCoroutine(SummonLoop());
        Debug.Log("Mahishasura Phase 2 - Summoning minions!");
    }

    void ActivateShield()
    {
        _shielded = true;
        _shieldCurrent = shieldHP;

        if (shieldVFXPrefab)
            _shieldVFX = Instantiate(shieldVFXPrefab, transform);
    }

    IEnumerator SummonLoop()
    {
        while (_phase == Phase.Two)
        {
            yield return new WaitForSeconds(summonCooldown);
            if (_phase != Phase.Two) break;
            SummonMinions();
        }
    }

    void SummonMinions()
    {
        if (minionPrefab == null) return;

        for (int i = 0; i < minionCount; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-1.5f, 1.5f), 0f, Random.Range(-1.5f, 1.5f));
            Instantiate(minionPrefab, transform.position + offset, Quaternion.identity);
        }
    }

    void EnterPhase3()
    {
        _phase = Phase.Three;
        _slowImmune = true;
        _enemy.speed *= berserkSpeedMult;

        if (_shielded)
            BreakShield();

        Debug.Log("Mahishasura Phase 3 - BERSERK!");
    }

    void BreakShield()
    {
        _shielded = false;
        if (_shieldVFX != null)
            Destroy(_shieldVFX);
    }

    float InterceptDamage(float damage)
    {
        if (_shielded)
        {
            _shieldCurrent -= damage;
            if (_shieldCurrent <= 0f)
            {
                float overflow = -_shieldCurrent;
                BreakShield();
                return overflow;
            }
            return 0f;
        }

        return damage;
    }

    public bool IsSlowImmune() => _slowImmune;
}
