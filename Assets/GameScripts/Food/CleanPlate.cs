using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CleanPlate : FoodContainer {

    public GameObject itemOnPlate = null;

    public GameObject cleanPlate;
    public GameObject dirtyPlate;

    [SyncVar]
    public bool isDirty = false;

    [SyncVar]
    public string itemOnPlateName = "";

    public override bool CanInsertItem(GameObject item)
    {
        bool can = false;

        if (isDirty)
        {
            return false;
        }

        if (itemOnPlate == null)
        {
            var f = item.GetComponent<FoodItem>().itemName;

            foreach (string s in allowedItems)
                if (f == s)
                    can = true;
        }
        else if (itemOnPlate != null)
        {

            if (itemOnPlate.GetComponent<FoodContainer>() != null)
            {
                var f = itemOnPlate.GetComponent<FoodContainer>();

                if (f.CanInsertItem(item))
                    can = true;

            }

        }
        return can;
    }

    public override void Start()
    {
        base.Start();

        if (itemOnPlateName != "")
            StartCoroutine(InitPlate(itemOnPlateName));
    }

    IEnumerator InitPlate(string s)
    {
        while (GameObject.Find(s) == null)
            yield return null;

        LocalInsertPlate(GameObject.Find(s));

        LocalSetPlateDirty(isDirty);
    }

    // SERVER ONLY! 
    public override void InsertFood(GameObject foodItem)
    {
        if (!isServer)
            return;

        if (itemOnPlate != null)
        {
            if (itemOnPlate.GetComponent<FoodContainer>() != null)
            {
                var f = itemOnPlate.GetComponent<FoodContainer>();

                if (f.CanInsertItem(foodItem))
                {
                    f.InsertFood(foodItem);
                    return;
                }
            }  
        }
        else if (itemOnPlate == null)
        {
            itemOnPlateName = foodItem.name;
            foodItem.GetComponent<FoodItem>().grabbedByName = name;
            RpcPlateFood(foodItem);
        }
    }

    // SERVER OVERRIDE TO CLEAR PLATE
    public override bool ClearContainer()
    {
        if (itemOnPlate != null)
        {
            NetworkServer.Destroy(itemOnPlate);
            itemOnPlateName = "";
            RpcClearPlate();
        }

        return true;
    }

    //SERVER ONLY
    public void SetPlateDirty(bool t)
    {
        if (t)
        {
            isDirty = true;
        }
        else
        {
            isDirty = false;
        }

        RpcSetPlateDirty(t);

    }

    [ClientRpc]
    public void RpcSetPlateDirty(bool t)
    {
        LocalSetPlateDirty(t);
    }

    [ClientRpc]
    public void RpcPlateFood(GameObject foodItem)
    {
        LocalInsertPlate(foodItem);

    }

    [ClientRpc]
    public void RpcClearPlate()
    {
        itemOnPlate = null;
        inContainer.Clear();
    }

    void LocalInsertPlate(GameObject foodItem)
    {
        inContainer.Add(foodItem.GetComponent<FoodItem>().itemName);

        foodItem.transform.SetParent(transform);
        foodItem.transform.position = transform.position;
        foodItem.transform.rotation = transform.rotation;

        var rb = foodItem.GetComponent<Rigidbody>();
        var c = foodItem.GetComponent<Collider>();
        var cc = foodItem.GetComponentsInChildren<Collider>();
        foodItem.GetComponent<Highlighter>().BrightenObject(GetComponent<Highlighter>().isHighlighted);

        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.isKinematic = true;

        foreach (Collider d in cc)
            Destroy(d);

        itemOnPlate = foodItem;

        Destroy(c);

        foodItem.GetComponent<FoodItem>().grabbedBy = gameObject;
    }

    void LocalSetPlateDirty(bool t)
    {
        foreach (MeshRenderer m in GetComponentsInChildren<MeshRenderer>())
        {
            m.enabled = true;
        }

        if (t)
        {
            cleanPlate.SetActive(false);
            dirtyPlate.SetActive(true);
        }
        else
        {
            dirtyPlate.SetActive(false);
            cleanPlate.SetActive(true);
        }
    }
   

}
