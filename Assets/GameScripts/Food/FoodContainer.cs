using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FoodContainer : FoodItem {

    // A list of allowed items in the container.
    public List<string> allowedItems = new List<string>();
    
    public List<string> inContainer = new List<string>();

    public virtual bool CanInsertItem(GameObject item)
    {
        //Override this. It's client-side checking only. 
        return true;
    }

    // RUNS AS A COMMAND
    public virtual void InsertFood(GameObject foodItem)
    {

    }

    // Server command to clear the container. Meant to be overridden.
    // returns a bool so we can check whether or not it should even run.
    // Default is false, don't run this function.
    public virtual bool ClearContainer()
    {
        return false;
    }


}
