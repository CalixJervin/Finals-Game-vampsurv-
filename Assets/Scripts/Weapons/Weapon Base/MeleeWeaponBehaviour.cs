using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponBehaviour : MonoBehaviour
{
    public WeaponScriptableObject weaponData;
    public float destroyAfterSeconds;

    protected float currentDamage;
    protected int currentPierce;
    protected float currentSpeed;
    protected float currentCooldownDuration;

    void Awake()
    {
        currentDamage = weaponData.Damage;
        currentPierce = weaponData.Pierce;
        currentSpeed = weaponData.Speed;
        currentCooldownDuration = weaponData.CooldownDuration;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            EnemyStats enemyStats = col.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(currentDamage, transform.position);
                
            }
        }
    }
}
