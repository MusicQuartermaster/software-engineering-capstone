using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitGame : MonoBehaviour
{
    // when the player clicks the Exit Game button
    void OnMouseUpAsButton() {
        // Exit the application
        // (Requirement 3.0.4)
        Application.Quit();
    }
}
