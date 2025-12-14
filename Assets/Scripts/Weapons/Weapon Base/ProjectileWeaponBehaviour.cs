using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base script of all projectile behaviours [To be placed on a prefab of a weapon that is a projectile]
/// </summary>
public class ProjectileWeaponBehaviour : MonoBehaviour
{
    public WeaponScriptableObject weaponData;
    protected Vector3 direction;
    public float destroyAfterSeconds;

    protected float currentDamage;
    protected int currentPierce;
    protected float currentSpeed;
    protected float currentCooldownDuration;

    public float rotationOffset = -45f;

    void Awake()
    {
        currentDamage = weaponData.Damage;
        currentPierce = weaponData.Pierce;
        currentSpeed = weaponData.Speed;
        currentCooldownDuration = weaponData.CooldownDuration;
    }

    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);

        SetDirectionTowardsClosestEnemy();
    }

    /*
    public void DirectionChecker(Vector3 dir)
    {
        direction = dir;

        float dirx = direction.x;
        float diry = direction.y;

        Vector3 scale = transform.localScale;
        Vector3 rotation = transform.rotation.eulerAngles;

        if (dirx < 0 && diry == 0) //left
        {
            scale.x = scale.x * -1;
            scale.y = scale.y * -1;
        }
        else if (dirx == 0 && diry < 0) //down
        {
            scale.y = scale.y * -1;
        }
        else if (dirx == 0 && diry > 0) //up
        {
            scale.x = scale.x * -1;
        }
        else if (dirx > 0 && diry > 0) //right up
        {
            rotation.z = 0f;
        }
        else if (dirx > 0 && diry < 0) //right down
        {
            rotation.z = -90f;
        }
        else if (dirx < 0 && diry > 0) //left up
        {
            scale.x = scale.x * -1;
            scale.y = scale.y * -1;
            rotation.z = -90f;
        }
        else if (dirx < 0 && diry < 0) //left down
        {
            scale.x = scale.x * -1;
            scale.y = scale.y * -1;
            rotation.z = 0f;
        }

        transform.localScale = scale;
        transform.rotation = Quaternion.Euler(rotation);    //Can't simply set the vector because cannot convert
    }
    */
    [HideInInspector] public Vector3 fallbackDirection;
    
    // Lagay kapag auto target
    void SetDirectionTowardsClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        // 1. Loop through all enemies to find the nearest one
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(enemy.transform.position, currentPos);
            if (distance < minDistance)
            {
                closestEnemy = enemy;
                minDistance = distance;
            }
        }

        // 2. If we found an enemy, calculate direction towards it
        if (closestEnemy != null)
        {
            // Vector math: Target Position - My Position = Direction
            Vector3 targetDir = (closestEnemy.transform.position - transform.position).normalized;
            direction = targetDir;
        }
        
        else
        {
           if (fallbackDirection == Vector3.zero)
            {
                direction = Vector3.right;
            }
            else
            {
                direction = fallbackDirection.normalized;
            }
        }

        RotateTowardsDirection(direction);
    }

    // NEW: Simpler rotation logic using Math instead of 8 if-statements
    void RotateTowardsDirection(Vector3 dir)
    {
        if (dir == Vector3.zero) return; // Prevent errors if not moving
        // Calculate the angle in degrees
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        
        // Apply rotation (This assumes your sprite is drawn facing RIGHT by default)
        transform.rotation = Quaternion.AngleAxis(angle + rotationOffset, Vector3.forward);
    }
    

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            EnemyStats enemyStats = col.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(currentDamage, transform.position);
                currentPierce--;
                if (currentPierce <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
