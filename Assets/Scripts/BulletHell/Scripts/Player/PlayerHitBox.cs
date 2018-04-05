using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitBox : MonoBehaviour 
{
    PlayerController mPlayerController;

    void Start()
    {
        mPlayerController = GetComponentInParent<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == TagManager.sSingleton.enemyTag || other.tag == TagManager.sSingleton.enemyBulletTag)
        {
            mPlayerController.GetDamaged();
            if(!mPlayerController.IsInvinsible) other.gameObject.SetActive(false);
        }
        else if (other.tag == TagManager.sSingleton.powerUpTag)
        {
            float val = other.GetComponent<PickUpValue>().value;
            mPlayerController.GetPowerUp(val); 
            other.gameObject.SetActive(false);
        }
        else if (other.tag == TagManager.sSingleton.scorePickUp)
        {
//            GameManager.sSingleton.AddScore();
            other.gameObject.SetActive(false);
        }
    }
}
