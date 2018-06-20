using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRControl : MonoBehaviour {

    [SerializeField]
    GameObject roboMesh;
    [SerializeField]
    GameObject vrCam;

    [SerializeField]
    GameObject leftHand;
    [SerializeField]
    GameObject rightHand;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

        // Set the robo body to match the VR direction
        roboMesh.transform.position = new Vector3(vrCam.transform.position.x, roboMesh.transform.position.y, vrCam.transform.position.z);
        roboMesh.transform.localEulerAngles = new Vector3(0,vrCam.transform.localEulerAngles.y,0);

        // Track the hands and set the position to the proper spots
        leftHand.transform.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
        rightHand.transform.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);

	}
}
