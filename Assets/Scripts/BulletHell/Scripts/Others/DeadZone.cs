using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour 
{
    void OnTriggerEnter2D(Collider2D other)
    {
//        if (other.transform.parent.tag == TagManager.sSingleton.playerBulletTag)
//        {
//            Debug.Log("Test");
//            other.transform.parent.gameObject.SetActive(false);
//        }
//        else 
        other.gameObject.SetActive(false);
    }
}
