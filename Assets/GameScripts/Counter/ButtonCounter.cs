using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class ButtonCounter : CounterItem {

    //Create a place we can insert a function
    public UnityEvent runOnPress;

    //RUN ON SERVERSIDE ONLY!
    public override void UseCounter(GameObject player, bool continuous, float deltaTime)
    {
        base.UseCounter(player, continuous, deltaTime);
        runOnPress.Invoke();
    }

}
