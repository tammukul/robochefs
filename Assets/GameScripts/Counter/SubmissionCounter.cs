using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SubmissionCounter : CounterItem {

    public GameObject refereeObj;
    public GameObject returnCounterObj;

    ItemReturn returnCounter;

    Referee referee;

    public override void Start()
    {
        returnCounter = returnCounterObj.GetComponent<ItemReturn>();
        referee = refereeObj.GetComponent<Referee>();
        base.Start();
    }

    public override void PlaceItem(GameObject foodItem)
    {
        if (foodItem.GetComponent<CleanPlate>() != null)
        {
            var p = foodItem.GetComponent<CleanPlate>();

            if (p.itemOnPlate != null)
            {

                if (referee.ExchangeForPoints(p.itemOnPlate))
                {

                    returnCounter.returnQ.Add(foodItem);
                    returnCounter.returnQTimes.Add(15f);

                    NetworkServer.Destroy(p.itemOnPlate);
                    p.itemOnPlateName = "";
                    p.RpcClearPlate();

                    RpcSoftDisableItem(foodItem);

                    RpcClearCounter();
                    return;
                }

            }

        }

        base.PlaceItem(foodItem);
    }

    [ClientRpc]
    void RpcSoftDisableItem(GameObject item)
    {

        var c = item.GetComponent<Collider>();
        var cc = item.GetComponentsInChildren<Collider>();
        var mr = item.GetComponentInChildren<MeshRenderer>();

        foreach (Collider d in cc)
            d.enabled = false;

        if (c != null)
            c.enabled = false;

        mr.enabled = false;

    }

}
