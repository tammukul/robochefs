using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Teleporter : CounterItem {

    // Set this in the inspector! :)
    public GameObject linkedTeleporter;

    [SyncVar]
    public float health = 10f;

    [SyncVar]
    public bool broken = false;

    public GameObject healthCanvas;
    public Image healthBar;

    float changeVelocity;

    public override void Start()
    {
        base.Start();

        placePos = transform.position + transform.up * 0.25f;

        healthBar.color = new Color(0, 0, 1f, 0.6f);

        UpdateHealth(health);
    }

    private void Update()
    {
        SmoothVal();
    }

    public override void UseCounter(GameObject player, bool cont, float deltaTime)
    {
        if (!broken && !linkedTeleporter.GetComponent<Teleporter>().broken && !cont)
        {
            TeleportItem(itemOnCounter, linkedTeleporter);

            // Create a random number by which we will
            // reduce the health of the teleporter
            float num = Random.Range(0, 2f);

            health -= num;

            health = Mathf.Clamp(health, 0, 10f);

            if (health == 0)
            {
                broken = true;
                RpcUpdateHealth(health);
                return;
            }

            RpcUpdateHealth(health);

            return;
        }
        
        if (broken && cont)
        {
            health += 3f * deltaTime;

            health = Mathf.Clamp(health, 0, 10f);

            if (health >= 10f)
            {
                RpcUpdateHealth(health);
                broken = false;
                return;
            }

            RpcUpdateHealth(health);

        }
    }

    //Runs as a command to teleport items
    public void TeleportItem(GameObject foodItem, GameObject linkedT)
    {

        var lt = linkedT.GetComponent<Teleporter>();

        GameObject send = null;
        GameObject recieve = null;

        if (lt.itemOnCounter != null)
            recieve = lt.itemOnCounter;

        if (itemOnCounter != null)
            send = itemOnCounter;

        if (recieve != null)
        {
            lt.itemOnCounterName = "";

            if (send == null)
                lt.RpcClearCounter();

            PlaceItem(recieve);
        }

        if (send != null)
        {
            itemOnCounterName = "";

            if (recieve == null)
                RpcClearCounter();

            lt.PlaceItem(send);
        }

    }

    [ClientRpc]
    void RpcUpdateHealth(float h)
    {

        UpdateHealth(h);

    }

    private void UpdateHealth(float h)
    {
        if (h == 0 && !broken)
        {
            continuousAction = true;
            broken = true;
        }

        if (h < 10 && broken)
        {
            continuousAction = true;
            healthBar.color = new Color(1f, 0, 0, 0.6f);
        }
        
        if (h >= 10)
        {
            continuousAction = false;
            healthBar.color = new Color(0, 0, 1f, 0.6f);
        }

        //healthBar.fillAmount = h / 10;
    }

    void SmoothVal()
    {
        float toVal = health/10f;
        float curVal = Mathf.SmoothDamp(healthBar.fillAmount, toVal, ref changeVelocity, 0.2f);
        healthBar.fillAmount = curVal;
    }
}
