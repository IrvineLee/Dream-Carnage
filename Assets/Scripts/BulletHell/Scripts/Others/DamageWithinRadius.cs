using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageWithinRadius : MonoBehaviour 
{
    public int damage;
    public float stayDuration = 0.25f;
//    public bool isOneTimeDamage = true;
	public int playerID = 1;
    public ParticleSystem explodeParticleSystem;

	List<Transform> mHitList = new List<Transform>();
	float mTimer;
    bool mIsFinishDamage = false;
	
	void Update () 
	{
		mTimer += Time.deltaTime;
		if (mTimer >= stayDuration) 
		{
			mTimer = 0;

            if (explodeParticleSystem.isPlaying) mIsFinishDamage = true;
            else
            {
                mIsFinishDamage = false;
                gameObject.SetActive (false);
            }
		}
	}

    public void PlayExplosion() { explodeParticleSystem.Play(); }

	void OnTriggerStay2D(Collider2D other)
	{
        if (mIsFinishDamage) return;

		for (int i = 0; i < mHitList.Count; i++) 
		{
			if (mHitList [i].transform == other.transform) return;
		}
		mHitList.Add (other.transform);
	}

	void OnDisable()
	{
		for (int i = 0; i < mHitList.Count; i++) 
		{
			if (mHitList [i].GetComponent<EnemyBase> () != null) 
			{
				EnemyBase enemyBase = mHitList [i].GetComponent<EnemyBase> ();
				enemyBase.isHitByMagnumRadius = false;
			}
		}
        mHitList.Clear();
	}
}
