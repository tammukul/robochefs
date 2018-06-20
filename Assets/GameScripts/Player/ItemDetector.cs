using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDetector : MonoBehaviour {

    public ActionHandler action;

    public LayerMask layer;

    LineRenderer line;

	// Use this for initialization
	void Awake () {

        // Set References
        action = GetComponentInParent<ActionHandler>();
        layer = LayerMask.GetMask("Default");

    }
	
	// Update is called once per frame
	void Update () {

        // Create a RaycastHit variable to store information about what was hit by the ray.
        RaycastHit floorHit;

        
        Vector3 origin = transform.position;
        float distance = 1.6f;
        Vector3 direction = ( transform.position + (transform.forward * 1f) - (transform.up * 0.45f) ) - transform.position;


        // Perform the raycast and if it hits something on the floor layer...
        if (Physics.Raycast(origin, direction, out floorHit, distance, layer))
        {

            if (floorHit.collider.gameObject.GetComponent<FoodItem>() != null)
            {

                if (action.itemInFocus != null)
                    action.itemInFocus.GetComponent<Highlighter>().BrightenObject(false);

                var f = floorHit.collider.gameObject.GetComponent<FoodItem>();

                action.continuousAction = f.continuousUse;

                action.itemInFocus = f.gameObject;
                action.itemInFocus.GetComponent<Highlighter>().BrightenObject(true);


            }
            else if (floorHit.collider.gameObject.GetComponent<CounterItem>() != null)
            {
                var c = floorHit.collider.gameObject.GetComponent<CounterItem>();

                if (action.counterInFocus != null)
                    action.counterInFocus.GetComponent<Highlighter>().BrightenObject(false);

                if (c.itemOnCounter != null)
                {
                    if (action.itemInFocus != null)
                        action.itemInFocus.GetComponent<Highlighter>().BrightenObject(false);

                    action.itemInFocus = c.itemOnCounter;
                }
                else action.itemInFocus = null;

                action.continuousAction = c.continuousAction;

                action.counterInFocus = c.gameObject;
                action.counterInFocus.GetComponent<Highlighter>().BrightenObject(true);

            }
            else if (floorHit.collider.gameObject.GetComponent<ActionHandler>() != null)
            {
                if (action.itemInFocus != null)
                    action.itemInFocus.GetComponent<Highlighter>().BrightenObject(false);

                var p = floorHit.collider.gameObject.GetComponent<ActionHandler>();

                if (p.itemInHands != null)
                {
                    action.itemInFocus = p.itemInHands;
                }

            }
        
        }
        else
        {

            if (action.itemInFocus != null)
            {
                action.itemInFocus.GetComponent<Highlighter>().BrightenObject(false);
                action.itemInFocus = null;
            }

            if (action.counterInFocus != null)
            {
                action.counterInFocus.GetComponent<Highlighter>().BrightenObject(false);
                action.counterInFocus = null;
            }

        }

    }

}
