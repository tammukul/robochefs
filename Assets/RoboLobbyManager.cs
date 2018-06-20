using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RoboLobbyManager : NetworkBehaviour {

    RoboNetManager netMan = null;

	// Use this for initialization
	void Start () {

        netMan = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<RoboNetManager>();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeLevel()
    {
        netMan.ServerChangeScene("Level1_1");
    }
}
