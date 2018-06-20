using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMovement : MonoBehaviour {

    public PlayerManager playMan;

    Vector3 camCenter;

    float minHeight = 11f;
    float centerRatio = -3.5f;

    // Distance from center to bounds
    float xDistanceRatio = 1.222f;
    float zDistanceRatio = 1.222f;

    float xPlayerDistance = 0;
    float zPlayerDistance = 0;

    float camHeight = 11f;
    float pX = 0;
    float pZ = 0;

    Vector3 playerCenter = new Vector3(0, 0, 0);

    private Vector3 velocity = Vector3.zero;
    private float fVelocity = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
        if (playMan != null)
        {
            // Stop this script if the player counter is at 0
            if (playMan.playerObjs.Count == 0)
                return;

            camHeight = transform.position.y;

            var changeHeight = false;

            // Add up all player positions
            foreach (GameObject p in playMan.playerObjs)
            {
                

                pX += p.transform.position.x;
                pZ += p.transform.position.z;

                if (Mathf.Abs(p.transform.position.x - playerCenter.x) > camHeight / xDistanceRatio)
                {
                    changeHeight = true;
                    camHeight = Mathf.Abs(p.transform.position.x - playerCenter.x) * xDistanceRatio;
                }

                if (Mathf.Abs(p.transform.position.z - playerCenter.z) > camHeight / zDistanceRatio)
                {
                    changeHeight = true;
                    camHeight = Mathf.Abs(p.transform.position.z - playerCenter.z) * zDistanceRatio;
                }

            }

            if (!changeHeight)
            {
                camHeight = Mathf.SmoothDamp(camHeight, minHeight, ref fVelocity, 0.05f);
            }

            // Get the average center between all players and set that as the new center
            playerCenter.x = pX / playMan.playerObjs.Count;
            playerCenter.z = pZ / playMan.playerObjs.Count;

            pX = 0;
            pZ = 0;

            // Set the center offset according to the height of the camera
            playerCenter.z += transform.position.y / centerRatio;

            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(playerCenter.x, camHeight, playerCenter.z), ref velocity, 0.4f);
        }

	}
}
