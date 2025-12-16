using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    void Awake()
    {
        // Singleton Pattern: Ensure only one MusicManager exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance); // Don't destroy this object when loading new scenes!
        }
        else
        {
            Destroy(gameObject); // If a new one tries to spawn, destroy it immediately
        }
    }
}