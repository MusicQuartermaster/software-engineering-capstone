using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManual : MonoBehaviour
{
    // when the player clicks the "New Game" button
    void OnMouseUpAsButton() {
        // load the How to Play menu
        // (Requirement 3.0.3)
        SceneManager.LoadScene("HowToPlayMenu");
    }
}
