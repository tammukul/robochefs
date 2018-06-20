using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TrashBin : CounterItem {

    // Override use function
    public override void UseCounter(GameObject player, bool cont, float deltaTime)
    {
        var p = player.GetComponent<ActionHandler>();

        if (p.itemInHands != null)
        {

            // If the item in their hands is a food container
            // Lets see if we can clear it off (if that function is available)
            if (p.itemInHands.GetComponent<FoodContainer>() != null)
            {
                if (p.itemInHands.GetComponent<FoodContainer>().ClearContainer())
                    return;
            }

            NetworkServer.Destroy(p.itemInHands);
            p.itemInHandsName = "";
            p.RpcClearHands();

        }
    }

}
