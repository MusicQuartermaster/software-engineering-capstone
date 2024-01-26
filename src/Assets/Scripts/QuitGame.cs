using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitGame : MonoBehaviour
{
    // when the player clicks the Quit Game button
    void OnMouseUpAsButton() {
        // returns to main menu
        // (Requirement 3.2.3)
        SceneManager.LoadScene("MainMenu");
    }
}
