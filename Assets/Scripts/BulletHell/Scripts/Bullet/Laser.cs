using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour 
{
    public int dmgPerFrame = 1;

	void OnTriggerStay2D(Collider2D other)
	{
		if (other.tag == TagManager.sSingleton.enemyBulletTag) 
		{
			other.gameObject.SetActive (false);
		}
	}
}
