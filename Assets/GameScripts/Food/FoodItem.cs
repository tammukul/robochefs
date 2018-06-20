using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class FoodItem : NetworkBehaviour {

    public string itemName;
    public bool cookable;
    public bool choppable;

    // For things like fire extinguishers and GUNS! :) 
    public bool useable;
    public bool continuousUse;

    // Class we want to call if we want to have a different
    // function called when we drop this item instead of the
    // default behavior. 
    public bool dropOverride = false;

    public GameObject cooksTo;
    public GameObject chopsTo;

    [SyncVar]
    public string grabbedByName = "";

    public GameObject grabbedBy = null;

    [HideInInspector]
    public NetworkIdentity netID;
    [HideInInspector]
    public Highlighter highlighter;

    public virtual void Start()
    {
        highlighter = GetComponent<Highlighter>();

        gameObject.name = itemName + GetComponent<NetworkIdentity>().netId.Value;


        // If the synced grabbedbyname is not null, run the init script function
        if (grabbedByName != "")
            StartCoroutine(initFoodItem(grabbedByName));

        var rb = GetComponent<Rigidbody>();

        netID = GetComponent<NetworkIdentity>();
        highlighter = GetComponent<Highlighter>();

        rb.mass = 0.5f;
    }

    // Function for new clients to initialize the food
    IEnumerator initFoodItem(string s)
    {
        while (GameObject.Find(s) == null)
        {
            yield return null;
        }

        LocalGetGrabbed(GameObject.Find(grabbedByName));
    }

    public virtual void FixedUpdate()
    {

        // If it's grabbed by a player, set the position accordingly.
        if (grabbedBy != null)
        {
            if (grabbedBy.GetComponent<ActionHandler>() != null)
            {
                transform.position = grabbedBy.transform.position + (grabbedBy.transform.forward * 0.7f) + (grabbedBy.transform.up * 0.3f);
                transform.rotation = grabbedBy.transform.rotation;

            }
        }
    }

    // FOR GETTING GRABBED!
    [ClientRpc]
    public void RpcGetGrabbed(GameObject player)
    {
        transform.SetParent(null);
        LocalGetGrabbed(player);
    }


    // Local function that runs on client when it gets grabbed. Gets called
    // by the RPC but it's separate so we can also call it locally when 
    // initializing the food item
    void LocalGetGrabbed(GameObject player)
    {
        var rb = GetComponent<Rigidbody>();
        var c = GetComponent<Collider>();
        var cc = GetComponentsInChildren<Collider>();
        highlighter.BrightenObject(false);

        rb.constraints = RigidbodyConstraints.FreezeAll;

        foreach (Collider d in cc)
            d.enabled = false;

        if (c != null)
            c.enabled = false;

        grabbedBy = player;
    }

    [ClientRpc]
    public void RpcClearGrabbed(Vector3 v)
    {
        var rb = GetComponent<Rigidbody>();
        var c = GetComponent<Collider>();
        var cc = GetComponentsInChildren<Collider>();
        highlighter.BrightenObject(false);

        rb.constraints = RigidbodyConstraints.None;

        foreach (Collider d in cc)
            d.enabled = true;

        c.enabled = true;

        grabbedBy = null;

        rb.velocity = v;

    }


    // SERVER ONLY

    // For an override function that will perform various actions.
    // DONT FORGET TO RETURN TRUE/FALSE SO WE KNOW WHETHER OR NOT TO
    // FALL BACK TO DEFAULT DROPPING/PLACEMENT BEHAVIOURS
    public virtual bool DropOverride(GameObject focusItem, GameObject focusCounter)
    {
        Debug.Log("CMD ON CLIENT ERROR: SOURCE IS :" + name);
        return false;
    }

    // A function to use the item, if it's something like
    // a fire extinguisher or GUN :)
    public virtual void UseItem(GameObject itemFocus, GameObject counterFocus)
    {

    }

}
