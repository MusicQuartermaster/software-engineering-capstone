using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNewGame : MonoBehaviour
{
    // when the player clicks the "New Game" button
    void OnMouseUpAsButton() {
        // load the main game as a scene
        // (Requirement 3.0.2)
        SceneManager.LoadScene("MainScene");
    }
}
