//This is our basic player controller!
//It allows us to move our player based on what keys we are pressing
//Note that any lines here that start with two slashes (//) are Comments
//Comments allow us to describe our code, but they are ignored by the computer


//This line let's our code interact with Unity, don't remove it!
using UnityEngine;

//Everything written after this line is logic that we can attach to our player!
public class PlayerMovement : MonoBehaviour
{
  //This variable, moveSpeed, determines how much our player moves each frame
  //"[SerializeField]" allows us to change this value inside of Unity
  //"int" is the type of value that we are storing, int means integer, or whole numbers
  //"moveSpeed" is the name of our variable. We can use it later in our code
  //We initialize moveSpeed with a value of 10
  //Note that most lines of our code MUST end with a semicolon (;), otherwise it won't work!
  [SerializeField] int moveSpeed = 10;

  //All of the code placed within "Update()" is excecuted once per frame
  //Here we check our input and use it to move move the player
  void Update()
  {
    //First we ask Unity if the right/left/up/down arrows key or WASD keys are being pressed
    //We store the left and right movement in a xAxisMovement variable
    float xAxisMovement = Input.GetAxisRaw("Horizontal");
    //We store the up and down movement in a zAxisMovement variable
    float zAxisMovement = Input.GetAxisRaw("Vertical");

    //We use our input variables to create a Vector3 called "moveDir"
    //A Vector3 represents x, y, z values, it's used everywhere in 3D games!
    Vector3 moveDir = new Vector3(xAxisMovement, 0, zAxisMovement);

    //This code makes sure the player doesn't move too fast diagonally - Don't worry about this for now!
    moveDir.Normalize();

    //Here we print out our new Vector3 to the console, so we can observe how our code is working
    print("Movement Vector3: " + moveDir);

    //Here we adjust our speed based on our frame rate
    //This ensures our game works the same across all different types of computers
    //Don't worry about this for now!
    float adjustedSpeed = moveSpeed * Time.deltaTime;

    //Almost Done!
    //Let's now multiply our moveDir by our adjusted speed and store this in another variable called "movement"
    Vector3 movement = moveDir * adjustedSpeed;

    //Lastly, we add our movement variable to the player's position
    transform.position = transform.position + movement;
  }
}
