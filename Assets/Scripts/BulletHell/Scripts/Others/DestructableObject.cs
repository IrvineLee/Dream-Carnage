using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour 
{
    public int hitPoint = 100;
    public float impactMultiplier = 1.0f;
    public float scoreMultiplier = 1.0f;

    PlayerController mPlayer1Controller, mPlayer2Controller;

    void Start()
    {
        mPlayer1Controller = GameManager.sSingleton.player1.GetComponent<PlayerController>();

        if (GameManager.sSingleton.player2 != null)
            mPlayer2Controller = GameManager.sSingleton.player2.GetComponent<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        string otherLayerName = LayerMask.LayerToName(other.gameObject.layer);
        if (otherLayerName == TagManager.sSingleton.playerBulletLayer)
        {
            if (other.tag == TagManager.sSingleton.player1BulletTag || other.tag == TagManager.sSingleton.player1BulletTag)
            {
                int damage = other.GetComponent<BulletMove>().GetBulletDamage;
                hitPoint -= damage;

                // Move object up slightly.
                Vector3 pos = transform.position;
                pos.y += Time.unscaledDeltaTime * impactMultiplier;
                transform.position = pos;

                // Update player's score.
                if (other.tag == TagManager.sSingleton.player1BulletTag)
                {
                    mPlayer1Controller.UpdateLinkBar();
                    mPlayer1Controller.UpdateScore((int)(damage * scoreMultiplier));
                }
                else if (other.tag == TagManager.sSingleton.player2BulletTag)
                {
                    mPlayer2Controller.UpdateLinkBar();
                    mPlayer2Controller.UpdateScore((int)(damage * scoreMultiplier));
                }

                // TODO : Effect it does when contact.
                other.gameObject.SetActive(false);

                // TODO : Effect it does when it dies.
                if (hitPoint <= 0) gameObject.SetActive(false);
            }
        }
    }
}
