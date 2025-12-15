using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Required to reload the scene

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel; // Reference to the UI Panel

    // Call this to turn on the screen
    public void TriggerGameOver()
    {
        gameOverPanel.SetActive(true); // Show the menu
        Time.timeScale = 0f; // Stop the game (Freeze time)
    }

    // Call this from the "Restart" button
    public void RestartGame()
    {
        Time.timeScale = 1f; // IMPORTANT: Unfreeze time before reloading!
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Call this from the "Menu" button
    public void ReturnToMenu()
    {
        Time.timeScale = 1f; // Unfreeze time
        SceneManager.LoadScene("Menu"); // Make sure your menu scene is named "Menu"
    }
}