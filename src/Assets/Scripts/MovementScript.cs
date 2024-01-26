using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MovementScript: MonoBehaviour {

  // information about the movement of the player
  public float jumpHeight = 10f;
  public float moveSpeed = 5.0f;
  public float grappleSpeed = 10f;
  public float airSpeed = 7.0f;
  public float maxRopeLength = 10.0f;
  public float ropeLength = 0.0f;
  public bool isGrappled = false;
  public float maxSpeed = 10.0f;

  // set player score as static to access it in other scripts
  // (Requirement 3.2.1)
  public static int playerScore = 0;
  public Text scoreText;

  public float direction = 0f;
  // store the tiles in a layermask to determine collisions (this was more as a way to future proof, not as necessary in this arcade mode, but it would be too difficult to revert this back. It works so I'm leaving it)
  public LayerMask ground;

  // Start is called before the first frame update
  void Start() {

    // get the hitbox (the area around the player that detects collisions and runs physics operations)
    // (Requirement 1.1.1)
    Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();

    // prevent main character from rotating
    rigidbody.freezeRotation = true;

    // manually increase the effect of gravity to make the game feel more satisfying
    rigidbody.gravityScale = 3.0f;

    // initially disable the grapple
    GetComponent<SpringJoint2D>().enabled = false; //physically
    GetComponent<LineRenderer>().enabled = false; // and visually

    //access every tile tagged as ground (A.K.A. every tile in the game)
    ground = LayerMask.GetMask("Ground");

    playerScore = 0;
  }

  // Update is called once per frame
  void Update() {
    // get the hitbox (the area around the player that detects collisions and runs physics operations)
    // (Requirement 1.1.1)
    Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
    // determine if the player is touching the ground beneath them
    // (Requirement 2.0.2)
    bool grounded = isGrounded();

    // if the player jumps, set the vertical velocity as the jump height and don't change the horizontal
    // only allow player to jump if they are not in mid-air
    // (Requirement 2.0.2)
    if (grounded && Input.GetKey("space")) {
      rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpHeight);
    }

    // when the user clicks, attempt to shoot a grapple from the player towards the direction of the mouse
    // (Requirement 2.0.3)
    if (Input.GetMouseButtonDown(0)) {
        grapple();
    }

    // when the user right clicks, disable the grapple
    // (Requirement 2.0.4)
    if (Input.GetMouseButtonDown(1)) {
        GetComponent<SpringJoint2D>().enabled = false;
        GetComponent<LineRenderer>().enabled = false;
        isGrappled = false;
    }

    // when the user presses escape, return to the main menu
    // (Requirement 3.4.0)
    if (Input.GetKey("escape")){
        SceneManager.LoadScene("MainMenu");
    }

    // enable the grapple if the player has successfully grappled a tile
    // (Requirement 2.0.3)
    if(isGrappled){
        GetComponent<LineRenderer>().enabled = true; // display the grapple
        GetComponent<LineRenderer>().SetPositions(new Vector3[]{
          rigidbody.transform.position, 
          GetComponent<SpringJoint2D>().connectedAnchor
        }); // set the start point of the grapple as the player's position, set the end point as the point where the grapple intersected with a block (if at all)
    }

    // horizontal movement direction (negative is left, zero is stationary, positive is right)
    // retrieved from the Unity project, allows multiple keys to dictate movements. A & D and Left & Right both work for horizontal movement.
    // (Requirement 2.0.1)
    direction = Input.GetAxis("Horizontal");

    // run different physics depending on if the player is on the ground, in the air, or grappled in the air
    if(grounded){
      // move the player when they press A or D, Left or Right
      rigidbody.AddForce(new Vector2(moveSpeed * direction * 10.0f, 0));
      // restrict the player's max speed in both directions (when on the ground) to make the game playable. Doesn't really matter in the arcade mode but I'd still rather it be here regardless.
      rigidbody.velocity = new Vector2(Mathf.Min(GetComponent<Rigidbody2D>().velocity.x, maxSpeed), GetComponent<Rigidbody2D>().velocity.y);
      rigidbody.velocity = new Vector2(Mathf.Max(GetComponent<Rigidbody2D>().velocity.x, -maxSpeed), GetComponent<Rigidbody2D>().velocity.y);
    }else if(isGrappled)
      // player can slightly control their character while in the air / grappled, increase their ability to do so while grappled
      rigidbody.AddForce(new Vector2(grappleSpeed * direction, 0));
    else
    {
      // make player move slightly faster in air relative to ground, but slower relative to grapple
      rigidbody.AddForce(new Vector2(airSpeed * direction, 0));
    }


    // use distance squared to save computation of the slow square root function.
    // mathematically consistant
      // suppose you have two points A and B, where A.x and B.x are the x-components of A and B, A.y and B.y are the y-components of A and B, and d is the distance between A and B
      // if 
      // d = sqrt((A.x - B.x)^2 + (A.y - B.y)^2)
      // then
      // d^2 = (A.x - B.x)^2 + (A.y - B.y)^2
    // the results are the same without needing the square root
    
    float distanceSquared = 0.0f;
    // calculate the distance between the player and the tile the player has grappled to
    distanceSquared = getDistanceSquared(GetComponent<SpringJoint2D>().connectedAnchor, rigidbody.transform.position);
    // if the distance is less than the length of the rope (determined as the initial grapple length), then disable the function of the grapple to mimic slack
    // (Requirement 2.2.1)
    if(isGrappled && distanceSquared < ropeLength * ropeLength){
      GetComponent<SpringJoint2D>().enabled = false;
    // (Requirement 2.2.2)
    }else if(isGrappled){
      GetComponent<SpringJoint2D>().enabled = true;
    }

    // when the player dalls down too far, the player dies
    // (Requirement 2.6.1)
    if(rigidbody.transform.position.y < -20){
      Kill();
    }


    // calculate the player's score based on the distance the player has traveled
    // (Requirement 2.5.0)
    playerScore = (int)Mathf.Max(playerScore, rigidbody.transform.position.x);
    scoreText.text = "Score: " + playerScore;

  }

  private float getDistanceSquared(Vector2 a, Vector2 b) {
    return ((a.y - b.y) * (a.y - b.y)) + ((a.x - b.x) * (a.x - b.x));
  }

  // draws a box straight down from the player and checks for collisions to determine if the player is on the ground
  // (Requirement 2.0.2)
  private bool isGrounded() {
    return Physics2D.BoxCast(GetComponent<Collider2D>().bounds.center, GetComponent<Collider2D>().bounds.extents * 2f, 0f, Vector2.down, 0.02f, ground);
  }

  // shoots a ray from the player towards the mouse and detects the first tile in the path. If the tile is close enough, attaches a grapple to it
  // (Requirement 2.0.3)
  private void grapple() {
    Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
    // get the coordinates of the mouse in world coordinates
    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    // calculate the direction from the player to the mouse
    float mousex = worldPosition.x - rigidbody.transform.position.x;
    float mousey = worldPosition.y - rigidbody.transform.position.y;
    Vector2 mouseDir = new Vector2(mousex, mousey);

    // cast a ray in that direction
    RaycastHit2D grapple = Physics2D.Raycast(rigidbody.transform.position, mouseDir, maxRopeLength, ground);

    // retrieve the point where the ray collided with a tile
    Vector2 collisionPoint = grapple.point;
    // if the fraction is 0, the grapple did not collide with a tile
    // either missed or the tile was farther than the max rope length (arbitrarily set to make the game more interesting)
    if (grapple.fraction == 0) {
      return;
    }

    // enable the grapple
    GetComponent<SpringJoint2D>().connectedBody = grapple.rigidbody; // set the grapple to start at the player
    GetComponent<SpringJoint2D>().connectedAnchor = collisionPoint; // set the grapple to end at the tile it connected to
    GetComponent<SpringJoint2D>().distance = grapple.distance; // set its distance and the rope length as the distance to the tile
    ropeLength = grapple.distance; // this is used to determine whether there should be "slack" in the grapple (it disables when the player gets too close, similar to having a string of elastic)

    // set the grapple locations to draw the grapple
    GetComponent<LineRenderer>().SetPositions(new Vector3[]{
        rigidbody.transform.position, 
        GetComponent<SpringJoint2D>().connectedAnchor
    });
    GetComponent<LineRenderer>().enabled = true; // enable the line on screen
    GetComponent<SpringJoint2D>().enabled = true; // enable the grapple itself
    isGrappled = true;
  }

  // displays the death screen, allows the player to quit the game or try again
  // (Requirement 2.6.0)
  // (Requirement 3.2.0)
  public void Kill(){
    SceneManager.LoadScene("DeathMenu");
  }
}