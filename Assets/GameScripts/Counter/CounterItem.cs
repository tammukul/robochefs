using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Highlighter))]
[RequireComponent(typeof(NetworkIdentity))]
public class CounterItem : NetworkBehaviour {

    [SyncVar]
    public string itemOnCounterName = "";

    public GameObject itemOnCounter = null;

    public bool continuousAction = false;

    public Vector3 placePos;

    // Special variable for counters so we can have items
    // pre-placed before the game starts. Avoids issues
    // when client joins. When server begins, it will be marked
    // as DIRTY
    [HideInInspector]
    [SyncVar]
    public bool isDirty = false;

    public virtual void Start()
    {
        gameObject.name = gameObject.name + GetComponent<NetworkIdentity>().netId;
        placePos = transform.position;

        if (itemOnCounterName != "")
        {
            StartCoroutine(InitCounter(itemOnCounterName));
        }

        if (!isDirty && isServer)
        {
            if (itemOnCounter != null)
                itemOnCounterName = itemOnCounter.GetComponent<FoodItem>().itemName + itemOnCounter.GetComponent<NetworkIdentity>().netId.Value;

            isDirty = true;
        }
        else if (isDirty)
        {
            if (itemOnCounterName == "")
                itemOnCounter = null;
        }
    }
    
    // Script to initialize counter top if the state does not match the beginning
    // state :)
    IEnumerator InitCounter(string s)
    {
        Debug.Log("Trying to find item " + s + " for the counter " + gameObject.name);
        while (GameObject.Find(s) == null)
        {
            yield return null;
        }

        GameObject.Find(s).SetActive(true);
        LocalPlaceItem(GameObject.Find(s));
    }

    // FOR ALL INTENTS AND PURPOSES, THIS IS A COMMAND EVEN THOUGH
    // IT DOESNT LOOK LIKE ONE K THX BAI
    public virtual void UseCounter(GameObject player, bool continuous, float deltaTime)
    {

    }

    // SERVER ONLY
    public virtual void PlaceItem(GameObject foodItem)
    {
        itemOnCounterName = foodItem.name;
        // Any command actions can go here before the RPC callback
        RpcPlaceItem(foodItem);
    }

    [ClientRpc]
    public void RpcClearCounter()
    {
        itemOnCounter = null;
    }

    [ClientRpc]
    public void RpcPlaceItem(GameObject foodItem)
    {
        LocalPlaceItem(foodItem);
    }

    public virtual void LocalPlaceItem(GameObject foodItem)
    {

        itemOnCounter = foodItem;

        // Client side stuff to do when the item gets placed down
        foodItem.transform.position = placePos;

        var f = foodItem.GetComponent<FoodItem>();
        var rb = foodItem.GetComponent<Rigidbody>();
        var c = foodItem.GetComponent<Collider>();
        var cc = foodItem.GetComponentsInChildren<Collider>();

        f.grabbedBy = gameObject;

        f.GetComponent<Highlighter>().BrightenObject(false);

        rb.constraints = RigidbodyConstraints.FreezeAll;

        foreach (Collider d in cc)
            d.enabled = false;

        c.enabled = false;
    }
}
