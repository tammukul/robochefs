using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JoyControl : NetworkBehaviour {

    public int playerNumber; // Player number set externally by the player manager

    string joyPrefix = ""; // The prefix so we know which buttons to listen for

    public float maxSpeed = 4f;      // The speed that the player will move at.

    Vector3 movement;                   // The vector to store the direction of the player's movement.
    Rigidbody playerRigidbody;

    ActionHandler action; // The action handler with lots of commands! :)

    float h = 0;
    float v = 0;
    float deadzone = 0.33f;
    Vector2 stickInput = new Vector2(0, 0);

    private void Start()
    {

        // Set up references
        playerRigidbody = GetComponent<Rigidbody>();
        action = GetComponent<ActionHandler>();

        // Get the player number assigned and set the Joypad Input Prefix
        switch (playerNumber)
        {
            case 1:
                joyPrefix = "P1 ";
                break;
            case 2:
                joyPrefix = "P2 ";
                break;
            case 3:
                joyPrefix = "P3 ";
                break;
            case 4:
                joyPrefix = "P4 ";
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    private void Update ()
    {

        if (!hasAuthority)
            return;

        if (!action.controlEnabled)
            return;

        // Store the input axes.
        h = Input.GetAxis(joyPrefix + "JoyH");
        v = Input.GetAxis(joyPrefix + "JoyV");


        // Our own deadzone implementation
        stickInput = new Vector2(h, v);
        if (stickInput.magnitude < deadzone)
            stickInput = Vector2.zero;
        else
            stickInput = stickInput.normalized * ((stickInput.magnitude - deadzone) / (1 - deadzone));

        // Only send the new output if the stick input is past the deadzone
        if (stickInput.x != 0 || stickInput.y != 0)
            SmoothLook(new Vector3(stickInput.x, 0, stickInput.y));

        //In the joypad controller, we add the joyPrefix (Which is a string set externally by the local player manager)
        // Button commands
        if (Input.GetButtonDown(joyPrefix + "Pick"))
        {
            action.GrabAction();
        }
        

        // Check to see if we should do single action or perform continuous action
        if (action.continuousAction)
        {
            if (Input.GetButton(joyPrefix + "Use"))
            {
                action.UseAction();
            }
        }
        else
        {
            if (Input.GetButtonDown(joyPrefix + "Use"))
            {
                action.UseAction();
            }
        }
        


    }

    void FixedUpdate()
    {

        if (!hasAuthority)
            return;

        // Move the player around the scene.
        Move(stickInput.x, stickInput.y);

    }


    // Function to smooth out the look
    void SmoothLook(Vector3 newDirection)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(newDirection), 15 * Time.deltaTime);
    }


    void Move(float h, float v)
    {

        movement = new Vector3(h, 0, v) * maxSpeed * Time.deltaTime;

        //Perform the move on the rigidbody
        playerRigidbody.MovePosition(transform.position + movement);

    }
}
