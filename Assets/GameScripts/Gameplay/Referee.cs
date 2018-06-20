using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Referee : NetworkBehaviour {

    // References
    public GameObject timerObj;
    public GameObject scoreObj;
    public RecipeGenerator recGen;
    public RoboNetManager netMan;

    Text timerText;
    Text scoreText;

    bool roundOver = false;

    public List<GameObject> spawnPoints = new List<GameObject>();

    // Float that we can use to count until we should add another recipe
    public float nextRecipeMin = 10f;
    public float nextRecipeMax = 20f;

    public int maximumRecipes = 5;

    float nextRecipe = 15f;

    [SyncVar]
    public float timeLeft = 99;

    [SyncVar]
    public int totalScore = 0;

	// Use this for initialization
	void Start () {
        timerText = timerObj.GetComponent<Text>();
        scoreText = scoreObj.GetComponent<Text>();
        scoreText.text = totalScore.ToString();
        recGen = GetComponent<RecipeGenerator>();

        //Get a reference to the network manager
        netMan = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<RoboNetManager>();

        if (isServer)
        {
            //Generate a recipe
            recGen.GenerateRecipe();

            //Spawn the players
            List<GameObject> matchPlayers = new List<GameObject>();

            //Create a temp list of all the players in the current match
            foreach (GameObject g in netMan.playerManagers)
            {
                g.GetComponent<PlayerManager>().InitCamGimbal();

                foreach (GameObject p in g.GetComponent<PlayerManager>().playerObjs)
                {
                    matchPlayers.Add(p);
                }
            }

            //See if there are any spawn points. If not, we won't take over spawning
            if (spawnPoints.Count == 0)
                return;

            //Temp count for spawning players
            int tempCount = 0;

            //Spawn the players
            for (int i = 0; i < matchPlayers.Count; i++)
            {
                if (i > spawnPoints.Count)
                {
                    tempCount += matchPlayers.Count;
                }
                matchPlayers[i].GetComponent<ActionHandler>().MovePlayer(spawnPoints[i - tempCount].transform.position);
            }

        }

    }
	
	// Update is called once per frame
	void Update () {

        if (!isServer || roundOver)
            return;

        // Run this is there's no time left
        if (timeLeft <= 0 && roundOver == false)
        {
            EndRound();
            roundOver = true;
        }

        // Subtract a second every second
        timeLeft -= Time.deltaTime;

        nextRecipe -= Time.deltaTime;

        timeLeft = Mathf.Clamp(timeLeft, 0, timeLeft);

        // Generate recipes every now and then.
        if (nextRecipe <= 0)
        {
            nextRecipe = Random.Range(nextRecipeMin, nextRecipeMax);
            // Check how many recipes are in play. We don't want more than 5 at a time.
            if (recGen.recipesInPlay.Count < maximumRecipes)
                recGen.GenerateRecipe();
        }

        RpcUpdateTimer(timeLeft);
		
	}

    // Converts plates to points!
    public bool ExchangeForPoints(GameObject foodItem)
    {

        var f = foodItem.GetComponent<FoodItem>();

        // Set this as true and if we need to we'll set it as false later.
        bool totalMatch = true;

        for (int i = 0; i < recGen.recipesInPlay.Count; i++)
        {
            totalMatch = true;

            var rec = recGen.recipesInPlay[i];

            if (rec.parentName != f.itemName)
                totalMatch = false;

            if (foodItem.GetComponent<FoodContainer>() != null && totalMatch)
            {
                var fc = foodItem.GetComponent<FoodContainer>();

                foreach (string s in rec.ingredients)
                {

                    bool subMatch = false;

                    foreach (string sub in fc.inContainer)
                    {

                        if (s == sub)
                            subMatch = true;

                    }

                    //Debug.Log("Does " + s + " match? " + subMatch);

                    if (!subMatch)
                        totalMatch = false;

                }

                if (rec.ingredients.Length != fc.inContainer.Count)
                    totalMatch = false;

            }

            // Lets mark it false if the number of ingredients IS NOT the same
            //Debug.Log(totalMatch);

            if (totalMatch)
            {
                int addToScore = 6 - i;

                recGen.recipesInPlay.RemoveAt(i);
                totalScore += addToScore;
                RpcUpdateScore(totalScore);

                return true;
            }

        }

        return totalMatch;

    }

    [ClientRpc]
    void RpcUpdateTimer(float time)
    {

        if (timerText != null)
            timerText.text = Mathf.Ceil(time).ToString();

    }

    [ClientRpc]
    void RpcUpdateScore(int score)
    {

        scoreText.text = score.ToString();

    }


    // Gets called at the end of the round
    void EndRound()
    {
        foreach (GameObject p in netMan.playerManagers)
        {
            // Call a server command to freeze the player
            CmdFreezePlayer(p, false);
        }
    }

    [Command]
    void CmdFreezePlayer(GameObject p, bool b)
    {
        p.GetComponent<PlayerManager>().EnableControls(b);
    }
}
