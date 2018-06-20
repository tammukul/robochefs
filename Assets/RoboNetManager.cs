using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RoboNetManager : NetworkManager {

    public GameObject UIGameObj;

    public List<GameObject> playerManagers = new List<GameObject>();

    public override void OnStartHost()
    {
        base.OnStartHost();
        UIGameObj.SetActive(false);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        UIGameObj.SetActive(false);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        UIGameObj.SetActive(true);
    }

    public override void OnStopHost()
    {
        base.OnStopHost();
        UIGameObj.SetActive(true);
    }
}
