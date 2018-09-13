using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertEnemyBullet : MonoBehaviour 
{
    public int bulletIndex = 0;
    int playerID;

    public int PlayerID { set { playerID = value; } }

	void OnTriggerStay2D(Collider2D other)
	{
		if (other.tag == TagManager.sSingleton.enemyBulletTag) 
		{
			other.gameObject.SetActive (false);

			Transform trans = other.transform;
            BulletManager.sSingleton.TransformEnemyBulIntoPlayerBul (playerID, trans, bulletIndex);

            if (AudioManager.sSingleton != null) AudioManager.sSingleton.PlayReflectedBulletSfx();
		}
	}
}
