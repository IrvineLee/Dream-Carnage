using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour 
{
    public int hitPoint = 100;
    public float impactMultiplier = 1.0f;
    public float scoreMultiplier = 1.0f;

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
                pos.y += Time.deltaTime * impactMultiplier;
                transform.position = pos;

                // Update player's score.
                int playerNum = 0;
                if (other.tag == TagManager.sSingleton.player1BulletTag) playerNum = 1;
                else if (other.tag == TagManager.sSingleton.player2BulletTag) playerNum = 2;
                UIManager.sSingleton.UpdateScore(playerNum, (int)(damage * scoreMultiplier));

                // TODO : Effect it does when contact.
                other.gameObject.SetActive(false);

                // TODO : Effect it does when it dies.
                if (hitPoint <= 0) gameObject.SetActive(false);
            }
        }
    }
}
