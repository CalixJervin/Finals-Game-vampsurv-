using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShovelBehaviour : MeleeWeaponBehaviour
{
    List<GameObject> markedEnemies;

    [Header("Swing Configuration")]
    public float radius = 1.5f;
    public float totalArc = 120f;
    public float weaponRotationOffset = -45f;

    // We no longer set this manually. It is calculated from Weapon Data.
    private float swingDuration; 

    private Vector3 fallbackDir;
    private float centerAngle;
    private float currentSwingTime;
    private float startAngle;
    private float endAngle;

    protected override void Start()
    {
        base.Start(); // IMPORTANT: This loads weaponData into currentSpeed, currentDamage, etc.
        
        markedEnemies = new List<GameObject>();
        currentSwingTime = 0f;

        // --- CONNECTING THE STATS ---
        
        // 1. SPEED to DURATION CONVERSION
        // Formula: Duration = (Base Constant / Speed Stat)
        // If Speed is 10, Duration is 0.25s. If Speed is 5, Duration is 0.5s.
        // Tweak the "2.5f" number to make the base feel right for your game.
        swingDuration = 2.5f / currentSpeed; 

        // ----------------------------

        CalculateAngles();
    }

    public void SetFallbackDirection(Vector3 dir)
    {
        fallbackDir = dir;
    }

    void CalculateAngles()
    {
        Vector3 dirToTarget = GetDirectionToTarget();
        centerAngle = Mathf.Atan2(dirToTarget.y, dirToTarget.x) * Mathf.Rad2Deg;
        
        // Randomize swing direction (Up-to-Down or Down-to-Up)
        bool swingClockwise = Random.value > 0.5f;

        float halfArc = totalArc / 2f;
        
        if (swingClockwise)
        {
            startAngle = centerAngle + halfArc;
            endAngle = centerAngle - halfArc;
        }
        else
        {
            startAngle = centerAngle - halfArc;
            endAngle = centerAngle + halfArc;
        }
    }

    void Update()
    {
        currentSwingTime += Time.deltaTime;
        float t = currentSwingTime / swingDuration; // 0.0 to 1.0

        // --- THE FIX ---
        // If the animation is finished (t >= 1), destroy object immediately.
        if (t >= 1f)
        {
            Destroy(gameObject);
            return; // Stop the code here so it doesn't try to move a destroyed object
        }
        // ----------------

        float currentAngle = Mathf.Lerp(startAngle, endAngle, t);

        // Convert Angle to Position
        float rad = currentAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * radius;

        transform.localPosition = offset;
        transform.rotation = Quaternion.Euler(0, 0, currentAngle + weaponRotationOffset);
    }

    Vector3 GetDirectionToTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPos = transform.parent.position;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(enemy.transform.position, currentPos);
            if (distance < minDistance)
            {
                closestEnemy = enemy;
                minDistance = distance;
            }
        }

        if (closestEnemy != null)
        {
            return (closestEnemy.transform.position - currentPos).normalized;
        }
        
        return (fallbackDir == Vector3.zero) ? Vector3.right : fallbackDir.normalized;
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        // Check if we hit an enemy AND if we haven't hit it yet
        if (col.CompareTag("Enemy") && !markedEnemies.Contains(col.gameObject))
        {
            // --- PIERCE CHECK ---
            // If we have already hit as many enemies as our 'Pierce' stat allows, stop hitting.
            if (markedEnemies.Count >= currentPierce) return;

            EnemyStats enemyStats = col.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                // Use the Damage from the Scriptable Object
                enemyStats.TakeDamage(currentDamage, transform.position);
                markedEnemies.Add(col.gameObject);
            }
        }
    }
}