using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour 
{
    public int hitPoint = 100;
    public float impactMultiplier = 1.0f;

    void OnTriggerEnter2D(Collider2D other)
    {
        string otherLayerName = LayerMask.LayerToName(other.gameObject.layer);
        if (otherLayerName == TagManager.sSingleton.playerBulletLayer)
        {
            if (other.tag == TagManager.sSingleton.player1BulletTag || other.tag == TagManager.sSingleton.player1BulletTag)
            {
                int damage = other.GetComponent<BulletMove>().GetBulletDamage;
                hitPoint -= damage;

                Vector3 pos = transform.position;
                pos.y += Time.deltaTime * impactMultiplier;
                transform.position = pos;

                // TODO : Effect it does when contact.
                other.gameObject.SetActive(false);

                if (hitPoint <= 0) gameObject.SetActive(false);
            }
        }
    }
}
