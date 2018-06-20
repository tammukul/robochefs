using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RecipeParser : NetworkBehaviour {

    public struct recipeInPlay
    {

        public string parentName;

        public List<string> ingredients;

        public GameObject recipeObject;

    }

    public List<recipeInPlay> rsInPlay = new List<recipeInPlay>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
