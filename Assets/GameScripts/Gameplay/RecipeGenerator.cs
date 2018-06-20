using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class RecipeGenerator : NetworkBehaviour {

    // The struct for defining ingredients in a recipe
    [System.Serializable]
    public struct ingredientStruct
    {
        [SerializeField]
        public string itemName;
        [SerializeField]
        public bool isRequired;
        [SerializeField]
        public string thumbnail;
    }

    // Struct for defining a recipe parent
    [System.Serializable]
    public struct foodRecipeItem
    {
        [SerializeField]
        public string foodContainerName;

        [SerializeField]
        public string thumbnail;

        [SerializeField]
        public List<ingredientStruct> ingredients;
    }

    // Struct we can use to send completed and ready to inject recipies.
    // ITS NETWORK READY OKAY?!?!?
    public struct injectionItem
    {

        public string parentName;
        public string parentThumbnail;

        public string[] ingredients;
        public string[] ingredientThumbnails;

    }

    // A list that contains all possible whole recipes
    // DEFINED IN THE INSPECTOR
    [SerializeField]
    public List<foodRecipeItem> wholeRecipes = new List<foodRecipeItem>();


    // UI REFERENCES
    // The UI gameobject which will be parent to the generated recipe UI items
    public GameObject recipeContainer;
    // We'll set the proper prefab
    public GameObject recipeContainerPrefab;

    HorizontalLayoutGroup hlg;

    // Just used by new Recipe prefabs to know where their transforms are supposed to go
    public GameObject recipeShellContainer;


    // NET SYNC STRUCT
    public class syncRPlay : SyncListStruct<injectionItem> { }
    [SyncVar]
    public syncRPlay recipesInPlay = new syncRPlay();


    // LOCAL LIST FOR RECIPES ADDED
    public List<GameObject> localRecipes = new List<GameObject>();



	// Use this for initialization
	void Start ()
    {
        hlg = recipeContainer.GetComponent<HorizontalLayoutGroup>();
        recipesInPlay.Callback = OnRecipeAdded;

        if (recipesInPlay.Count > 0)
        {
            for (int i = 0; i<recipesInPlay.Count; i++)
            {
                LocalAddRecipe(i);
            }
        }

        if (!isServer)
            return;



        //GenerateRecipe();

    }

    public void GenerateRecipe()
    {

        // This creates the UI element for the given recipe
        int recipeToGenerateNum = Random.Range(0, wholeRecipes.Count - 1);

        var inUse = wholeRecipes[recipeToGenerateNum];

        injectionItem toInject = new injectionItem();

        toInject.parentName = inUse.foodContainerName;
        toInject.parentThumbnail = inUse.thumbnail;

        List<string> tempIngredientsName = new List<string>();
        List<string> tempIngredients = new List<string>();

        foreach (ingredientStruct i in inUse.ingredients)
        {

            if (i.isRequired)
            {
                tempIngredientsName.Add(i.itemName);
                tempIngredients.Add(i.thumbnail);
            }
            else
            {
                int rand = Random.Range(0, 2);

                if (rand == 1)
                {
                    tempIngredientsName.Add(i.itemName);
                    tempIngredients.Add(i.thumbnail);
                }

            }

        }

        toInject.ingredients = new string[tempIngredientsName.Count];
        toInject.ingredientThumbnails = new string[tempIngredients.Count];

        for (var i = 0; i < tempIngredients.Count; i++)
        {

            toInject.ingredients[i] = tempIngredientsName[i];
            toInject.ingredientThumbnails[i] = tempIngredients[i];

        }

        recipesInPlay.Add(toInject);

    }


    void OnRecipeAdded(syncRPlay.Operation op, int index)
    {
        //Debug.Log(op.ToString() + " "+ index);

        if (op.ToString() == "OP_ADD")
        {
            //Debug.Log("ADDED STUFF!!!");
            LocalAddRecipe(index);
        }
        else if (op.ToString() == "OP_REMOVEAT")
        {
            //Debug.Log("Removing Item " + index);
            LocalRemoveRecipe(index);
        }

    }


    void LocalAddRecipe(int index)
    {
        injectionItem i = recipesInPlay[index];

        var newObj = Instantiate(recipeContainerPrefab);

        newObj.transform.SetParent(recipeContainer.transform, false);

        RecipeListingInjector inj = newObj.GetComponent<RecipeListingInjector>();

        inj.parentImage = i.parentThumbnail;

        inj.shellContainer = recipeShellContainer;

        foreach(string spritename in i.ingredientThumbnails)
        {
            inj.ingredients.Add(spritename);
        }

        // Add it to the local view so we can refer to it later
        localRecipes.Add(newObj);

    }

    void LocalRemoveRecipe(int index)
    {

        localRecipes[index].GetComponent<RecipeListingInjector>().DestroyThis();
        localRecipes.RemoveAt(index);

    }

    // Update is called once per frame
    void Update () {
        LayoutRebuilder.ForceRebuildLayoutImmediate(recipeShellContainer.transform as RectTransform);
    }
}
