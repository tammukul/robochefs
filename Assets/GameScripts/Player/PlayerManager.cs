using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerManager : NetworkBehaviour {

    public GameObject playerPrefab;

    public RoboNetManager netMan;

    [SyncVar]
    int playerCount;

    public List<GameObject> playerObjs = new List<GameObject>();

    bool pk = false;
    bool p1 = false;
    bool p2 = false;
    bool p3 = false;
    bool p4 = false;

    GameObject cam;
    int camCheck = 0;

	// Use this for initialization
	void Start ()
    {

        // References here
        netMan = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<RoboNetManager>();

        if (netMan != null)
        {
            netMan.playerManagers.Add(this.gameObject);

            gameObject.transform.SetParent(netMan.gameObject.transform);
        }

        // Called when the object first is initialized
        InitCamGimbal();

        Debug.Log("PlayerManagerStart");

    }

    // Initialize the cam gimbal with this function
    public void InitCamGimbal()
    {
        if (hasAuthority)
        {
            var g = GameObject.FindGameObjectWithTag("CamGimbal").GetComponent<CamMovement>();

            cam = g.gameObject;
            g.playMan = this;
            return;
        }

        camCheck += 1;
    }

    // Update is called once per frame
    void Update () {

        if (!isLocalPlayer)
            return;

        if (Input.GetButtonDown("Left Click") && !pk && playerCount < 4)
        {
            CmdSpawnPlayer(0, gameObject);
        }

        if (Input.GetButtonDown("P1 Use") && !p1 && playerCount < 4)
        {
            CmdSpawnPlayer(1, gameObject);
        }

        if (cam == null && camCheck < 10)
        {
            InitCamGimbal();
        }

    }

    public void EnableControls(bool b)
    {
        RpcEnableControls(b);
    }

    [ClientRpc]
    void RpcEnableControls(bool b)
    {
        foreach (GameObject g in playerObjs)
        {
            g.GetComponent<ActionHandler>().controlEnabled = b;
        }
    }

    [Command]
    void CmdSpawnPlayer(int i, GameObject player)
    {
        var spawnPoint = new Vector3(0, 5f, 0);

        if (GameObject.FindGameObjectWithTag("GameMaster") != null)
        {
            var gm = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<Referee>();
            spawnPoint = gm.spawnPoints[Random.Range(0, gm.spawnPoints.Count)].transform.position;
        }

        var obj = Instantiate(playerPrefab, spawnPoint, transform.rotation);

        NetworkServer.SpawnWithClientAuthority(obj, player);

        playerCount += 1;

        RpcSetupPlayer(obj, i);

        obj = null;
    }

    [ClientRpc]
    void RpcSetupPlayer(GameObject player, int i)
    {
        switch (i)
        {
            case 0:
                pk = true;
                player.GetComponent<KeyboardControl>().enabled = true;
                Destroy(player.GetComponent<JoyControl>());
                break;
            case 1:
                player.GetComponent<JoyControl>().playerNumber = 1;
                player.GetComponent<JoyControl>().enabled = true;
                Destroy(player.GetComponent<KeyboardControl>());
                break;
            default:
                break;
        }

        player.GetComponent<ActionHandler>().netID = GetComponent<NetworkIdentity>();

        playerObjs.Add(player);      

        if (!hasAuthority)
        {
            Destroy(player.GetComponentInChildren<ItemDetector>());
        }

        player.transform.SetParent(gameObject.transform);

    }

}
