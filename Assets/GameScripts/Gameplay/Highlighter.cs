using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Highlighter : MonoBehaviour {

    public bool isHighlighted = false;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void BrightenObject(bool b)
    {

        var rends = GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in rends)
        {

            foreach (Material m in rend.materials)
            {
                if (b && !isHighlighted)
                {
                    m.color += new Color(0.3f, 0.3f, 0.3f);
                    
                }    
                else if (!b && isHighlighted)
                {
                    m.color -= new Color(0.3f, 0.3f, 0.3f);
                }
                    
            }

        }

        if (b && !isHighlighted)
            isHighlighted = true;
        else if (!b && isHighlighted)
            isHighlighted = false;

    }

}
