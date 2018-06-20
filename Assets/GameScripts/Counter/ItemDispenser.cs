using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ItemDispenser : CounterItem {

    public GameObject itemToDispense;

    bool waitTrigger = false;

    public override void UseCounter(GameObject player, bool cont, float deltaTime)
    {
        //Debug.Log("CMD ON CLIENT ERROR: SOURCE IS :" + name);

        // Make sure the counter is blank before dispensing something
        if (itemOnCounter == null && itemToDispense != null && !waitTrigger)
        {
            var obj = Instantiate(itemToDispense);
            obj.transform.position = transform.position;

            // Spawn the item to dispense
            NetworkServer.Spawn(obj);

            //Set its position
            obj.transform.position = transform.position + new Vector3(0, 0.1f, 0);

            // Set its name to the proper format. (Food Item Name + NET ID VALUE)
            obj.name = obj.GetComponent<FoodItem>().itemName + obj.GetComponent<NetworkIdentity>().netId.Value;

            // Set the itemoncounter name to be properly set
            itemOnCounterName = obj.name;

            // Perform the local placing function
            PlaceItem(obj);

            RpcDispenserCallback(player, obj);

            StartCoroutine(WaitTrigger());
        }
    }
    
    // Cooldown script
    IEnumerator WaitTrigger()
    {

        waitTrigger = true;
        yield return new WaitForSeconds(0.5f);
        waitTrigger = false;

    }

    [ClientRpc]
    void RpcDispenserCallback(GameObject player, GameObject newFood)
    {
        player.GetComponent<ActionHandler>().itemInFocus = newFood;
    }

}
