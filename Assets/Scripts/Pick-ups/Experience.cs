using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Experience : Pickup, ICollectible
{
    public int expGranted;
    public int points;

    public void Collect()
    {
        Debug.Log("Collected EXP");
        PlayerStats player = FindObjectOfType<PlayerStats>();
        player.IncreaseExperience(expGranted);
        player.IncreaseScore(points);
    }

    
}
