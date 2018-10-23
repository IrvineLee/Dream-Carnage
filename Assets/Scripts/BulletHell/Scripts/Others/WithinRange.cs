using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WithinRange : MonoBehaviour 
{
	public List<Transform> hitList = new List<Transform>();

    Transform mPlayerTrans;

	void Start () 
    {
        mPlayerTrans = transform.parent;
	}

    public Transform GetClosestEnemy()
    {
        if (hitList.Count == 0) return null;

        Transform closestTrans = hitList[0];
        float minSqrLength = (mPlayerTrans.position - hitList[0].position).sqrMagnitude;

        int count = hitList.Count;
        for (int i = 1; i < count; i++)
        {
			if (i > hitList.Count - 1) break;

            if (!hitList[i].gameObject.activeSelf)
            {
                hitList.Remove(hitList[i].transform);
                continue;
            }

            float magnitude = (mPlayerTrans.position - hitList[i].position).sqrMagnitude;
            if (magnitude < minSqrLength)
            {
                closestTrans = hitList[i];
                minSqrLength = magnitude;
            }
        }

        return closestTrans;
    }

    public Transform GetFurthestEnemy()
    {
        if (hitList.Count == 0) return null;

        Transform furthestTrans = hitList[0];
        float maxSqrLength = (mPlayerTrans.position - hitList[0].position).sqrMagnitude;

        int count = hitList.Count;
        for (int i = 1; i < count; i++)
        {
            if (!hitList[i].gameObject.activeSelf)
            {
                hitList.Remove(hitList[i].transform);
                continue;
            }

            float magnitude = (mPlayerTrans.position - hitList[i].position).sqrMagnitude;
            if (magnitude > maxSqrLength)
            {
                furthestTrans = hitList[i];
                maxSqrLength = magnitude;
            }
        }

        return furthestTrans;
    }

    public void ResetHitList() { hitList.Clear(); }

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == TagManager.sSingleton.enemyTag || other.tag == TagManager.sSingleton.ENV_OBJ_RockTag || 
			other.tag == TagManager.sSingleton.ENV_OBJ_CrateTag) 
        {
			hitList.Add (other.transform);
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		hitList.Remove (other.transform);
	}
}
