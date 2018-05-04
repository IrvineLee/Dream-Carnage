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
        if (other.tag == TagManager.sSingleton.enemyBulletTag)
        {
            mPlayerController.GetDamaged();
            if(!mPlayerController.IsInvinsible) other.gameObject.SetActive(false);
        }
        else if (other.tag == TagManager.sSingleton.ENV_OBJ_PowerUp1Tag || other.tag == TagManager.sSingleton.ENV_OBJ_PowerUp2Tag)
        {
            float val = other.GetComponent<EnvironmentalObject>().value;
            mPlayerController.GetPowerUp(val); 
            other.gameObject.SetActive(false);
        }
        else if (other.tag == TagManager.sSingleton.ENV_OBJ_ScorePickUpTag)
        {
//            GameManager.sSingleton.AddScore();
            other.gameObject.SetActive(false);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == TagManager.sSingleton.ENV_OBJ_DamagePlayerTag) mPlayerController.GetDamaged();
    }
}
