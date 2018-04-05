using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour 
{
    public int hitPoint = 100;

    void OnTriggerEnter2D(Collider2D other)
    {
        string layerName = LayerMask.LayerToName(other.gameObject.layer);
        if (layerName == TagManager.sSingleton.playerBulletLayer)
        {
            int damage = other.GetComponent<BulletMove>().GetBulletDamage;
            hitPoint -= damage;
        }
    }
}
