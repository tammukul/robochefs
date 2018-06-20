using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FryingPan : FoodContainer {

    [SyncVar]
    public string itemOnPanName = "";

    public GameObject itemOnPan = null;

    public override void Start()
    {
        base.Start();

        if (itemOnPanName != "")
            StartCoroutine(InitPan(itemOnPanName));

        dropOverride = true;
    }

    IEnumerator InitPan(string s)
    {
        while (GameObject.Find(s) == null)
        {
            yield return null;
        }

        Debug.Log("Inserting Item FOUNDS");
        LocalInsertFood(GameObject.Find(s));
    }

    //Basically a command
    public override bool DropOverride(GameObject focusItem, GameObject focusCounter)
    {
        var d = false;

        if (focusItem != null && itemOnPan != null)
        {
            if (focusItem.GetComponent<FoodContainer>() != null)
            {
                if (focusItem.GetComponent<FoodContainer>().CanInsertItem(itemOnPan))
                {
                    focusItem.GetComponent<FoodContainer>().InsertFood(itemOnPan);
                    itemOnPanName = "";
                    RpcClearPan();
                    d = true;
                }

            }
        }

        return d;
    }

    // Command to clear frying pan
    public override bool ClearContainer()
    {
        if (itemOnPan != null)
        {
            NetworkServer.Destroy(itemOnPan);
            itemOnPanName = "";
            RpcClearPan();
        }

        return true;
    }

    //Client side test to check if allowed
    public override bool CanInsertItem(GameObject item)
    {
        bool can = false;

        if (itemOnPan == null)
        {
            var f = item.GetComponent<FoodItem>().itemName;

            foreach (string s in allowedItems)
                if (f == s)
                    can = true;
        }
        else if (itemOnPan != null)
        {

            can = false;

        }
        return can;
    }

    public override void InsertFood(GameObject foodItem)
    {
        //Debug.Log("CMD ON CLIENT ERROR: SOURCE IS :" + name);

        itemOnPanName = foodItem.GetComponent<FoodItem>().itemName + foodItem.GetComponent<NetworkIdentity>().netId.Value;
        foodItem.GetComponent<FoodItem>().grabbedByName = name;

        RpcInsertFood(foodItem);
    }

    [ClientRpc]
    void RpcInsertFood(GameObject foodItem)
    {
        LocalInsertFood(foodItem);
    }

    [ClientRpc]
    void RpcClearPan()
    {
        itemOnPan = null;
    }

    void LocalInsertFood(GameObject foodItem)
    {
        foodItem.transform.SetParent(transform);

        foodItem.transform.position = transform.position;
        foodItem.transform.rotation = transform.rotation;

        var rb = foodItem.GetComponent<Rigidbody>();
        var c = foodItem.GetComponent<Collider>();
        var cc = foodItem.GetComponentsInChildren<Collider>();
        foodItem.GetComponent<Highlighter>().BrightenObject(GetComponent<Highlighter>().isHighlighted);

        //foodItem.GetComponent<NetworkTransform>().transformSyncMode = NetworkTransform.TransformSyncMode.SyncNone;

        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.isKinematic = true;

        //Destroy(rb);

        Destroy(c);
        foreach (Collider d in cc)
            Destroy(d);

        itemOnPan = foodItem;
        foodItem.GetComponent<FoodItem>().grabbedBy = gameObject;

        
    }
}
