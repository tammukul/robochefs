using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ActionHandler : NetworkBehaviour {

    // Local variables only
    public GameObject itemInFocus = null;
    public GameObject counterInFocus = null;

    public NetworkIdentity netID;

    public bool continuousAction = false;
    public bool controlEnabled = true;

    [SyncVar]
    public string itemInHandsName = "";

    public GameObject itemInHands = null;

    public bool init = false;

    Vector3 velocity;
    Vector3 oneFrameAgo;

    // INIT
    private void Start()
    {
        gameObject.name = gameObject.name + GetComponent<NetworkIdentity>().netId;

        netID = GetComponent<NetworkIdentity>();

        if (itemInHandsName != "")
            StartCoroutine(initialize());

    }

    // This script waits until everything is loaded properly to initialize fully.
    IEnumerator initialize()
    {
        Debug.Log("Waiting until an item is found.");
        while (GameObject.Find(itemInHandsName) == null)
        {
            yield return null;
        }

        Debug.Log("Object found!");

        LocalGrabItem(GameObject.Find(itemInHandsName), null);
    }

    // UPDATES
    private void FixedUpdate()
    {
        // Set velocity for use with items so they can be tossed
        velocity = transform.position - oneFrameAgo;
        oneFrameAgo = transform.position;
    }

    // LOCAL ACTIONS CALLED
    public void GrabAction()
    {
        
        if (itemInHands == null)
        {

            if (itemInFocus != null)
                CmdGrabItem(itemInFocus, gameObject, counterInFocus);

        }
        else if (itemInHands != null)
        {
            CmdPlaceItem(itemInHands, itemInFocus, counterInFocus);
        }

    }

    public void UseAction()
    {

        if (itemInHands != null)
            if (itemInHands.GetComponent<FoodItem>().useable)
            {
                CmdUseHandItem(itemInHands, itemInFocus, counterInFocus);
                return;
            }

        if (counterInFocus != null)
        {
            CmdUseCounter(counterInFocus, gameObject, continuousAction, Time.deltaTime);
            return;
        }

    }

    // GRAB ITEMS
    [Command]
    void CmdGrabItem(GameObject foodItem, GameObject player, GameObject counterFocus)
    {

        var f = foodItem.GetComponent<FoodItem>();

        if (counterFocus != null && f.grabbedBy == counterFocus)
        {
            counterFocus.GetComponent<CounterItem>().itemOnCounterName = "";
            counterFocus.GetComponent<CounterItem>().RpcClearCounter();
        }

        f.RpcGetGrabbed(player);
        f.grabbedByName = player.name;
        itemInHandsName = foodItem.name;
        itemInHands = foodItem;
        RpcGrabItem(foodItem, counterFocus);

    }

    [ClientRpc]
    void RpcGrabItem(GameObject foodItem, GameObject counterFocus)
    {
        LocalGrabItem(foodItem, counterFocus);
        if (foodItem == itemInFocus)
            itemInFocus = null;

    }

    void LocalGrabItem(GameObject foodItem, GameObject counterFocus)
    {
        var f = foodItem.GetComponent<FoodItem>();
        itemInHands = foodItem;

        if (f.continuousUse)
            continuousAction = true;

        if (counterFocus != null)
        {
            if (counterFocus.GetComponent<CounterItem>().itemOnCounter == foodItem)
                counterFocus.GetComponent<CounterItem>().itemOnCounter = null;
        }
            
    }


    // PLACE ITEMS
    [Command]
    void CmdPlaceItem(GameObject foodItem, GameObject itemFocus, GameObject counterFocus)
    {

        var f = foodItem.GetComponent<FoodItem>();

        if (f.dropOverride)
        {
            if (f.DropOverride(itemFocus, counterFocus))
            {
                return;
            }
        }

        if (itemFocus != null && itemFocus != foodItem)
        {
            if (itemFocus.GetComponent<FoodContainer>() != null)
            {
                var c = itemFocus.GetComponent<FoodContainer>();

                if (c.CanInsertItem(foodItem))
                {
                    itemInHandsName = "";
                    f.grabbedByName = "";

                    //Running this as a command!
                    c.InsertFood(foodItem);
                    RpcClearHands();
                    return;
                }
            }

        }

        else if (counterFocus != null)
        {
            var c = counterFocus.GetComponent<CounterItem>();

            if (c.itemOnCounter == null && c.itemOnCounter != foodItem)
            {
                f.grabbedByName = counterFocus.name;
                c.itemOnCounterName = itemInHands.name;
                itemInHandsName = "";
                //f.RpcClearGrabbed(Vector3.zero);

                c.PlaceItem(foodItem);
                RpcClearHands();
                
                return;
            }

        }

        DropItem(foodItem);
        RpcClearHands();

    }

    //COMMAND TO DROP ITEM (ONLY RUN FROM CMD)
    void DropItem(GameObject foodItem)
    {
        var f = foodItem.GetComponent<FoodItem>();

        f.grabbedByName = "";
        itemInHandsName = "";

        //Clear the grabbed item and send the new velocity
        f.RpcClearGrabbed(velocity*40);
    }

    //COMMAND TO MOVE PLAYER
    [Server]
    public void MovePlayer(Vector3 v)
    {
        gameObject.transform.position = v;
        RpcMovePlayer(v);
    }

    [ClientRpc]
    void RpcMovePlayer(Vector3 v)
    {
        gameObject.transform.position = v;
    }

    // CLEAR HANDS
    [ClientRpc]
    public void RpcClearHands()
    {
        itemInHands = null;
        continuousAction = false;
    }

    // USE ITEM
    [Command]
    void CmdUseHandItem(GameObject itemHands, GameObject itemFocus, GameObject counterFocus)
    {
        var f = itemHands.GetComponent<FoodItem>();

        // Running as a command, basically.
        f.UseItem(itemFocus, counterFocus);
    }

    // USE COUNTER
    [Command]
    void CmdUseCounter(GameObject counterFocus, GameObject player, bool cont, float deltaTime)
    {
        var c = counterFocus.GetComponent<CounterItem>();

        // Running as a command, basically.
        c.UseCounter(player, cont, deltaTime);
    }


}
