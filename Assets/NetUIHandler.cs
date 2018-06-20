using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetUIHandler : MonoBehaviour {

    [SerializeField]
    GameObject mainMenuObj;

    [SerializeField]
    NetworkManager netMan;

    [SerializeField]
    Text hostPort;

    [SerializeField]
    Text hostIP;

    [SerializeField]
    Text clientIP;

    [SerializeField]
    Text clientPort;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () { 

	}

    public void StartAsHost()
    {

        if (hostIP.text != "")
        {
            netMan.networkAddress = hostIP.text;
            netMan.networkPort = int.Parse(hostPort.text);
        }
        else
        {
            netMan.networkAddress = "localhost";
            netMan.networkPort = 27000;
        }

        netMan.StartHost();

        Debug.Log("Starting as Host");
      
    }

    public void ConnectToGame()
    {
        

        if (clientIP.text != "")
        {
            netMan.networkAddress = clientIP.text;
            netMan.networkPort = int.Parse(clientPort.text);
        }
        else
        {
            netMan.networkAddress = "73.170.106.2";
            netMan.networkPort = 27000;
        }

        netMan.StartClient();

        Debug.Log("Connecting to Game");

    }
}
