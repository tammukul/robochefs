using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeListingInjector : MonoBehaviour {

    public string parentImage;

    public List<string> ingredients = new List<string>();

    public Image parentImageContainer;

    public GameObject ingredientContainer;

    public GameObject shellContainer;

    GameObject shellObj;

    Vector3 refVel;

    RectTransform r;
    RectTransform innerR;

    Animator animat;

	// Use this for initialization
	void Start () {

        animat = GetComponent<Animator>();

        innerR = GetComponent<RectTransform>();

        parentImageContainer.sprite = Resources.Load<Sprite>("FoodSprites/" + parentImage) as Sprite;

        foreach(string spriteN in ingredients)
        {
            Sprite s = Resources.Load<Sprite>("FoodSprites/" + spriteN);

            GameObject newIObj = new GameObject();
            Image newIngredient = newIObj.AddComponent<Image>();
            newIngredient.sprite = s;

            newIObj.transform.SetParent(ingredientContainer.transform);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(ingredientContainer.transform as RectTransform);

        shellObj = new GameObject();

        shellObj.name = name + " shell";

        shellObj.AddComponent<RectTransform>();

        r = shellObj.GetComponent<RectTransform>();

        r.pivot = new Vector2(0, 0);

        shellObj.transform.SetParent(shellContainer.transform, false);

    }
	
	// Update is called once per frame
	void Update () {

        if (shellObj == null)
            return;

        Vector3 toPos = shellObj.transform.position;
        Vector3 newPos = Vector3.SmoothDamp(transform.position, toPos, ref refVel, 0.4f);
        transform.position = newPos;

        r.sizeDelta = innerR.sizeDelta;

    }

    public void DestroyThis()
    {
        animat.Play("UIFadeOut");
    }

    private void OnDestroy()
    {
        Destroy(shellObj);
    }
}
