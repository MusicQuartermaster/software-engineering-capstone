using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class LoadScore : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // when the player dies, retrieve the static score value from the MovementScript, since values cannot be passed across scenes
        // (Requirement 3.2.1)
        TextMesh scoreText = GetComponent<TextMesh>();
        scoreText.text += " " + MovementScript.playerScore;
    }
}
