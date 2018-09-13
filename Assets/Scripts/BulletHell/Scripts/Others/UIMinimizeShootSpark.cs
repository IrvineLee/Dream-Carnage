using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMinimizeShootSpark : MonoBehaviour 
{
    public float speed;

    Vector3 mDefaultScale;

	void Start () 
    {
        mDefaultScale = transform.localScale;
	}
	
	void Update () 
    {
        Vector3 scale = transform.localScale;
        scale.x -= Time.deltaTime * speed;
        scale.y -= Time.deltaTime * speed;
        transform.localScale = scale;

        if (scale.x <= 0 || scale.y <= 0)
        {
            transform.localScale = mDefaultScale;
            gameObject.SetActive(false);
        }
	}
}
