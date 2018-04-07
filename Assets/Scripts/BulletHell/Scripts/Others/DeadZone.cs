using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour 
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "CapsuleCollider") return;
        other.gameObject.SetActive(false);

//        string otherTag = other.transform.parent.tag;
//        if (otherTag == TagManager.sSingleton.player1BulletTag || otherTag == TagManager.sSingleton.player2BulletTag ||
//            otherTag == TagManager.sSingleton.enemyBulletTag || otherTag == TagManager.sSingleton.enemyTag)
//        {
//            other.gameObject.SetActive(false);
//        }
    }
}
