using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BurgerBun : FoodContainer {

    public GameObject topBun;
    public GameObject bottomBun;

    public GameObject tomatoObj;
    public GameObject beefObj;
    public GameObject lettuceObj;
    public GameObject cheeseObj;

    [SyncVar]
    public bool tomato = false;
    [SyncVar]
    public bool beef = false;
    [SyncVar]
    public bool lettuce = false;
    [SyncVar]
    public bool cheese = false;

    public override void Start()
    {
        base.Start();

        if (tomato || beef || lettuce || cheese)
            InitBurger();
    }

    void InitBurger()
    {
        if (tomato)
        {
            tomatoObj.GetComponent<MeshRenderer>().enabled = true;
            inContainer.Add("Sliced Tomato");
        }

        if (beef)
        {
            beefObj.GetComponent<MeshRenderer>().enabled = true;
            inContainer.Add("Cooked Burger");
        }

        if (lettuce)
        {
            lettuceObj.GetComponent<MeshRenderer>().enabled = true;
            inContainer.Add("Sliced Lettuce");
        }

        if (cheese)
        { 
            cheeseObj.GetComponent<MeshRenderer>().enabled = true;
            inContainer.Add("Sliced Cheese");
        }

        UpdateBurger();

    }

    public override bool CanInsertItem(GameObject item)
    {

        switch (item.GetComponent<FoodItem>().itemName)
        {
            case "Sliced Tomato":
                if (tomato)
                    return false;
                else return true;
            case "Sliced Lettuce":
                if (lettuce)
                    return false;
                else return true;
            case "Sliced Cheese":
                if (cheese)
                    return false;
                else return true;
            case "Cooked Burger":
                if (beef)
                    return false;
                else return true;
            default:
                return false;
        }

    }

    // Runs as a command on the server
    public override void InsertFood(GameObject foodItem)
    {

        //Debug.Log("CMD ON CLIENT ERROR: SOURCE IS :" + name);

        var fname = foodItem.GetComponent<FoodItem>().itemName;

        switch (fname)
        {
            case "Sliced Tomato":
                tomato = true;
                break;
            case "Sliced Lettuce":
                lettuce = true;
                break;
            case "Sliced Cheese":
                cheese = true;
                break;
            case "Cooked Burger":
                beef = true;
                break;
            default:
                break;
        }

        inContainer.Add(fname);

        RpcUpdateFood(fname);
        NetworkServer.Destroy(foodItem);

        
    }

    [ClientRpc]
    public void RpcUpdateFood(string fname)
    {

        switch (fname)
        {
            case "Sliced Tomato":
                tomatoObj.GetComponent<MeshRenderer>().enabled = true;
                break;
            case "Sliced Lettuce":
                lettuceObj.GetComponent<MeshRenderer>().enabled = true;
                break;
            case "Sliced Cheese":
                cheeseObj.GetComponent<MeshRenderer>().enabled = true;
                break;
            case "Cooked Burger":
                beefObj.GetComponent<MeshRenderer>().enabled = true;
                break;
            default:
                break;
        }

        UpdateBurger();

    }

    void UpdateBurger()
    {

        var lastPos = bottomBun.transform.localPosition;

        if (beef)
        {
            beefObj.GetComponent<MeshRenderer>().enabled = true;
            beefObj.transform.transform.localPosition = lastPos;
            lastPos = beefObj.transform.localPosition;
        }
        else lastPos -= new Vector3(0, 0.08f, 0);

        if (cheese)
        {
            cheeseObj.GetComponent<MeshRenderer>().enabled = true;
            cheeseObj.transform.transform.localPosition = lastPos;
            lastPos = cheeseObj.transform.localPosition;
        }
        else lastPos -= new Vector3(0, 0.02f, 0);

        if (tomato)
        {
            tomatoObj.GetComponent<MeshRenderer>().enabled = true;
            tomatoObj.transform.transform.localPosition = lastPos;
            lastPos = tomatoObj.transform.localPosition;
        }
        else lastPos -= new Vector3(0, 0.01f, 0);

        if (lettuce)
        {
            lettuceObj.GetComponent<MeshRenderer>().enabled = true;
            lettuceObj.transform.transform.localPosition = lastPos;
            lastPos = lettuceObj.transform.localPosition;
        }
        else lastPos -= new Vector3(0, 0.01f, 0);

        topBun.transform.localPosition = lastPos;

    }
    
}
