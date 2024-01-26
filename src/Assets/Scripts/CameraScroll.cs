using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraScroll : MonoBehaviour
{
    // access the position of the player
    public Transform player;

    // access the tilemap in order to place the procedurely generated tiles
    public Tilemap tilemap;
    public Tile tile;
    
    // store information about the tile positions to make sure they show up in the right place at the right time
    private float prevPosition;
    private float minHeight, maxHeight;
    private float tileWidth = 1.0f;

    // store space between the tiles, starting at the right space
    // (Requirement 2.4.1)
    // private float screenWidthBuffer = 11.0f;
    // (Requirement 2.4.2)
    private float tileSpacer = 4.0f;

    //control how quickly the camera scrolls
    private float step = 0.04f;
    
    
    // Start is called before the first frame update
    void Start()
    {
        prevPosition = 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
        // store the camera's position in a vector
        // this needs to be an implicit variable declaration or the game doesn't work. I do not know why.
        var cameraPosition = Camera.main.gameObject.transform.position;
        cameraPosition.x += step;

        // calculate the min and max height of the possible tile placements so that they are always close to the player
        // (Requirement 2.4.1)
        minHeight = player.position.y - 2.0f;
        maxHeight = player.position.y + 10.0f;

        // speed up camera to keep up with player if they start moving too far to the right
        // (Requirement 2.3.2)
        if(player.position.x - cameraPosition.x >= 3){
            cameraPosition.x = player.position.x - 3;
        }

        // if the player lags too far behind, the player loses
        // (Requirement 2.6.2)
        if(cameraPosition.x - player.position.x >= 15){
            player.GetComponent<MovementScript>().Kill();
        }
        // slowly speed up the camera to make the game more difficult
        // (Requirement 2.3.1)
        step *= 1.0001f;

        // keep the camera focused on the player's height
        // (Requirement 2.3.3)
        cameraPosition.y = player.position.y + 3;

        // set the camera's position to reflect the changes made to it
        Camera.main.gameObject.transform.position = cameraPosition;

        // once the camera has moved far enough to the right, generate a new random tile placement
        // (Requirement 2.4.0)
        if(cameraPosition.x >= prevPosition + tileWidth * tileSpacer){
            addTile();
        }
    }

    private void addTile(){
        // get random tile height
        // (Requirement 2.4.1)
        Vector2 cameraPosition = Camera.main.gameObject.transform.position;
        float nextHeight = Random.Range(minHeight, maxHeight);

        // get the exact grid position of the random height and x position just off screen
        Vector3Int currentCell = tilemap.WorldToCell(new Vector2(cameraPosition.x + tileWidth * tileSpacer + 12.0f, nextHeight));
        
        // set the tile at that grid location
        tilemap.SetTile(currentCell, tile);
        prevPosition = cameraPosition.x;
    }
}
