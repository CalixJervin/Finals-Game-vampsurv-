using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeController : WeaponController
{

    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();

        // 1. Spawn the knife
        GameObject spawnedKnife = Instantiate(weaponData.Prefab, transform.position, Quaternion.identity);
        
        // 2. Get the projectile script
        ProjectileWeaponBehaviour knifeScript = spawnedKnife.GetComponent<ProjectileWeaponBehaviour>();

        // 3. FIND THE PLAYER'S FACING DIRECTION
        // Assuming your PlayerMovement script has the 'lastMovedVector' variable we discussed
        PlayerMovement pm = FindObjectOfType<PlayerMovement>();
        
        if (pm != null && knifeScript != null)
        {
            // PASS THE DIRECTION TO THE KNIFE
            knifeScript.fallbackDirection = pm.lastMovedVector;
        }
    }
}
