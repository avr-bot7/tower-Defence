using System.Collections;
using UnityEngine;

public class RakshasaEnemy : MonoBehaviour
{
    [Header("Armour")]
    [Range(0f, 0.9f)]
    public float damageReduction = 0.4f;

    [Header("Enrage")]
    public float enrageThreshold = 0.3f;
    public float enrageSpeedMult = 1.8f;
    public float enrageDuration = 5f;

    [Header("VFX")]
    public GameObject armorBreakVFX;
    public GameObject enrageVFX;

    Enemy _enemy;
    bool _enraged;
    bool _armorStripped;

    void Start()
    {
        _enemy = GetComponent<Enemy>();
        if (_enemy != null)
            _enemy.OnTakeDamage += ApplyArmour;
    }

    void OnDestroy()
    {
        if (_enemy != null)
            _enemy.OnTakeDamage -= ApplyArmour;
    }

    void Update()
    {
        if (_enraged || _enemy == null || _enemy.maxHP <= 0f) return;

        if (_enemy.hp / _enemy.maxHP <= enrageThreshold)
            StartCoroutine(Enrage());
    }

    float ApplyArmour(float incomingDamage)
    {
        if (_armorStripped) return incomingDamage;
        return incomingDamage * (1f - damageReduction);
    }

    IEnumerator Enrage()
    {
        _enraged = true;
        _armorStripped = true;

        if (armorBreakVFX)
            Instantiate(armorBreakVFX, transform.position, Quaternion.identity);
        if (enrageVFX)
            Instantiate(enrageVFX, transform.position, Quaternion.identity);

        float originalSpeed = _enemy.speed;
        _enemy.speed *= enrageSpeedMult;

        yield return new WaitForSeconds(enrageDuration);

        if (_enemy != null)
            _enemy.speed = originalSpeed;
    }
}
