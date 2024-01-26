using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAgainHover : MonoBehaviour
{
    // (Requirement 3.5.0)

    // when the player hovers their mouse over the Play Again button
    void OnMouseEnter()
    {
        // change its color to magenta
	    GetComponent<Renderer>().material.color = Color.magenta;
        // and enable the bright neon lights around it
        GameObject.Find("Play Again Outline").GetComponent<MeshRenderer>().enabled = true;
    }

    void OnMouseExit()
    {
        // change its color back to cyan
	    GetComponent<Renderer>().material.color = Color.cyan;
        // and disable the bright neon lights around it
        GameObject.Find("Play Again Outline").GetComponent<MeshRenderer>().enabled = false;
    }
}
