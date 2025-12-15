using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required to talk to the Slider

public class HealthBarScript : MonoBehaviour
{
    public Slider slider; // Reference to the UI Slider

    // Call this when the game starts (to set the size of the bar)
    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    // Call this whenever the player gets hurt
    public void SetHealth(float health)
    {
        slider.value = health;
    }
}