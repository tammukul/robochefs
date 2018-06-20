using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class StoveCounter : CounterItem
{

    [SyncVar]
    float cookProgress = 0;

    public Image progressBar;
    public GameObject progressCanvas;

    public float cookSpeed = 1.2f;

    bool checkCook = false;
    float changeVelocity;

    // OVERRIDE PLACEMENT POSITION HERE
    public override void Start()
    {

        // Here we override the placement position
        base.Start();
        placePos = transform.position + transform.up * 0.1f - transform.forward * 0.1f;
    }

    private void Update()
    {
        if (hasAuthority && checkCook)
            CmdCookItem();

        checkCook = CheckCookable(itemOnCounter);

        SmoothVal();
    }


    // Function for checking if the item on the counter has a cookable item within!
    bool CheckCookable(GameObject foodItem)
    {
        bool r = false;

        if (foodItem == null)
        {
            progressCanvas.SetActive(false);
            return false;
        }
            

        if (foodItem.GetComponent<FryingPan>() != null)
        {
            var pan = foodItem.GetComponent<FryingPan>();

            if (pan.itemOnPan != null)
            {
                if (pan.itemOnPan.GetComponent<FoodItem>().cookable)
                    r = true;
            }
        }
        return r;

    }

    // SERVER ONLY
    public override void PlaceItem(GameObject foodItem)
    {
        //Debug.Log("CMD ON CLIENT ERROR: SOURCE IS :" + name);
        cookProgress = 0;
        base.PlaceItem(foodItem);
    }

    [Command]
    void CmdCookItem()
    {

        cookProgress += cookSpeed * Time.deltaTime;

        cookProgress = Mathf.Clamp(cookProgress, 0, 10f);

        if (cookProgress >= 10f)
        {
            // Code here to complete cooking
            cookProgress = 0;

            var f = itemOnCounter.GetComponent<FryingPan>();
            var n = Instantiate(f.itemOnPan.GetComponent<FoodItem>().cooksTo);
            var o = f.itemOnPan;

            n.transform.position = transform.position;

            NetworkServer.Destroy(f.itemOnPan);
            NetworkServer.Spawn(n);

            //PUT THE NEW ITEM ON THE PAN HERE PLS
            f.InsertFood(n);

            // RPC Callback to destroy the serverside Item
            RpcCompleteCooking(o, n);
            return;
        }

        RpcUpdateCooking(cookProgress);
    }

    [ClientRpc]
    void RpcUpdateCooking(float value)
    {
        if (value > 0)
        {
            progressCanvas.SetActive(true);
            
        }
        else progressCanvas.SetActive(false);

        // progressBar.fillAmount = value / 10f;
    }


    [ClientRpc]
    void RpcCompleteCooking(GameObject oldFood, GameObject newFood)
    {
        checkCook = CheckCookable(newFood);
        progressBar.fillAmount = 0;
        progressCanvas.SetActive(false);
        Destroy(oldFood);
    }

    public override void LocalPlaceItem(GameObject foodItem)
    {
        base.LocalPlaceItem(foodItem);
        checkCook = CheckCookable(foodItem);
    }

    void SmoothVal()
    {
        float toVal = cookProgress / 10f;
        float curVal = Mathf.SmoothDamp(progressBar.fillAmount, toVal, ref changeVelocity, 0.2f);
        progressBar.fillAmount = curVal;

        if (cookProgress == 0 || itemOnCounter == null)
            progressCanvas.SetActive(false);
    }

}