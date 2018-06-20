using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class KeyboardControl : NetworkBehaviour {

    public float maxSpeed = 4f;      // The speed that the player will move at.

    Vector3 movement;                   // The vector to store the direction of the player's movement.
    Rigidbody playerRigidbody;          // Reference to the player's rigidbody.
    int floorMask;                      // A layer mask so that a ray can be cast just at gameobjects on the floor layer.
    float camRayLength = 100f;          // The length of the ray from the camera into the scene.

    ActionHandler action;

    void Start ()
    {
        // Create a layer mask for the floor layer.
        floorMask = LayerMask.GetMask ("Floor");

        // Set up references.
        playerRigidbody = GetComponent <Rigidbody>();
        action = GetComponent<ActionHandler>();

    }


    void FixedUpdate ()
    {
        if (!hasAuthority)
            return;

        if (!action.controlEnabled)
            return;

        // Store the input axes.
        float h = Input.GetAxis ("KeyHorizontal");
        float v = Input.GetAxis ("KeyVertical");

        // Move the player around the scene.
        Move (h, v);

        // Turn the player to face the mouse cursor.
        Turning ();

    }


    // If you are ever wondering why you are having issues with dropped button presses, it's because it's not in here!
    private void Update()
    {
        if (!hasAuthority)
            return;

        // Other actions
        if (Input.GetButtonDown("Left Click"))
        {
            action.GrabAction();
        }

        // Check to see if we should do single action or perform continuous action
        if (action.continuousAction)
        {
            if (Input.GetButton("Right Click"))
            {
                action.UseAction();
            }
        }
        else
        {
            if (Input.GetButtonDown("Right Click"))
            {
                action.UseAction();
            }
        }

    }

    void Move (float h, float v)
    {

        //Set the movement vector to use as an offset
        movement = new Vector3(h, 0, v) * maxSpeed * Time.deltaTime;

        

        //Perform the move on the rigidbody
        playerRigidbody.MovePosition(transform.position + movement);

    }

    void Turning ()
    {
        // Create a ray from the mouse cursor on screen in the direction of the camera.
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Create a RaycastHit variable to store information about what was hit by the ray.
        RaycastHit floorHit;

        // Perform the raycast and if it hits something on the floor layer...
        if(Physics.Raycast(camRay, out floorHit, camRayLength, floorMask))
        {
            // Create a vector from the player to the point on the floor the raycast from the mouse hit.
            Vector3 playerToMouse = floorHit.point - transform.position;

            // Ensure the vector is entirely along the floor plane.
            playerToMouse.y = 0f;

            // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
            Quaternion newRotation = Quaternion.LookRotation(playerToMouse);

            // Set the player's rotation to this new rotation.
            playerRigidbody.MoveRotation(newRotation);
        }
    }
    
}
