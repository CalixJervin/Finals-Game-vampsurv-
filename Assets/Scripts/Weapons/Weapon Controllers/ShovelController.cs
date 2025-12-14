using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShovelController : WeaponController
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedShovel = Instantiate(weaponData.Prefab);
        
        // Parent it to the player so it moves with you
        spawnedShovel.transform.parent = transform; 
        
        // Reset local position so calculations are accurate
        spawnedShovel.transform.localPosition = Vector3.zero;

        // Get the script and pass the data
        ShovelBehaviour shovelScript = spawnedShovel.GetComponent<ShovelBehaviour>();
        
        // Find Player direction (Safe check)
        PlayerMovement pm = FindObjectOfType<PlayerMovement>();
        if (pm != null && shovelScript != null)
        {
            // Assuming your PlayerMovement has the 'lastMovedVector' we made earlier
            shovelScript.SetFallbackDirection(pm.lastMovedVector);
    }
}
}