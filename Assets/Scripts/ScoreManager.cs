using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Needed for TextMeshPro

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance; // The Key to finding this script
    public TMP_Text scoreText; // Reference to the UI Text

    void Awake()
    {
        // Set up the singleton
        if (instance == null)
        {
            instance = this;
        }
    }

    public void UpdateScoreDisplay(float currentScore)
    {
        // Update the text on screen
        scoreText.text = "Score: " + currentScore.ToString();
    }
}