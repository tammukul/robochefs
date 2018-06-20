using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ItemReturn : CounterItem {

    public List<GameObject> returnQ = new List<GameObject>();
    public List<float> returnQTimes = new List<float>();

	// Update is called once per frame
	void Update ()
    {

        if (!isServer)
            return;

        if (itemOnCounter != null || returnQ.Count <= 0)
            return;

        for (int i = 0; i < returnQ.Count; i++)
        {

            returnQTimes[i] -= Time.deltaTime;

            if (returnQTimes[i] <= 0 && itemOnCounter == null)
            {
                GameObject dirtyObj;

                dirtyObj = returnQ[0];

                dirtyObj.GetComponent<CleanPlate>().SetPlateDirty(true);

                returnQ.RemoveAt(0);
                returnQTimes.RemoveAt(0);

                RpcMakeVisible(dirtyObj);

                dirtyObj.GetComponent<FoodItem>().grabbedByName = name;

                PlaceItem(dirtyObj);

                

                return;
            }

            if (returnQTimes[i] <= 0 && itemOnCounter != null)
            {

                returnQTimes[i] += 3f;

            }

        }
		
	}

    [ClientRpc]
    void RpcMakeVisible(GameObject item)
    {

        var c = item.GetComponent<Collider>();
        var cc = item.GetComponentsInChildren<Collider>();
        var mr = item.GetComponentInChildren<MeshRenderer>();

        foreach (Collider d in cc)
            d.enabled = true;

        if (c != null)
            c.enabled = true;

        mr.enabled = true;

    }

    public override void LocalPlaceItem(GameObject foodItem)
    {
        base.LocalPlaceItem(foodItem);
    }
}
