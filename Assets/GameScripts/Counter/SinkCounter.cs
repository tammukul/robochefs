using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SinkCounter : CounterItem {

    [SyncVar]
    float washProgress = 0;

    public Image progressBar;
    public GameObject progressCanvas;

    public float washSpeed = 4f;

    float changeVelocity;


    // INIT SCRIPTS
    public override void Start()
    {
        base.Start();
        continuousAction = true;

        if (washProgress > 0)
            InitSinkWashing(washProgress);

    }

    void InitSinkWashing(float p)
    {
        progressCanvas.SetActive(true);
        progressBar.fillAmount = washProgress / 10f;
    }


    // LIVE UPDATE SCRIPTS
    private void Update()
    {
        SmoothVal();
    }


    // COMMANDS
    public override void PlaceItem(GameObject foodItem)
    {
        washProgress = 0;
        base.PlaceItem(foodItem);
    }

    // COMMAND TO USE COUNTER
    public override void UseCounter(GameObject player, bool cont, float deltaTime)
    {
        if (itemOnCounter == null)
        {
            washProgress = 0;
            RpcUpdateProgress(washProgress);
            return;
        }

        if (itemOnCounter != null)
        {
            if (itemOnCounter.GetComponent<CleanPlate>() == null)
                return;

            if (itemOnCounter.GetComponent<CleanPlate>().isDirty == false)
                return;
        }

        washProgress += washSpeed * Time.deltaTime;

        washProgress = Mathf.Clamp(washProgress, 0, 10f);

        if (washProgress >= 10f)
        {
            itemOnCounter.GetComponent<CleanPlate>().SetPlateDirty(false);

            washProgress = 0;
            RpcUpdateProgress(washProgress);
            return;
        }

        RpcUpdateProgress(washProgress);
    }

    [ClientRpc]
    void RpcUpdateProgress(float value)
    {
        if (value > 0)
        {
            progressCanvas.SetActive(true);
        }
        else progressCanvas.SetActive(false);

        // progressBar.fillAmount = value / 10f;
    }

    void SmoothVal()
    {
        float toVal = washProgress / 10f;
        float curVal = Mathf.SmoothDamp(progressBar.fillAmount, toVal, ref changeVelocity, 0.2f);
        progressBar.fillAmount = curVal;

        if (washProgress == 0 || itemOnCounter == null)
            progressCanvas.SetActive(false);
    }
}
