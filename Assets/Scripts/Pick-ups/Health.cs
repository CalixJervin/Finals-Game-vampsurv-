using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : Pickup, ICollectible
{
    public int healthGained;

    public void Collect()
    {
        PlayerStats player = FindObjectOfType<PlayerStats>();
        player.IncreaseHealth(healthGained);
    }
}
