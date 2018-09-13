using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletWipe : MonoBehaviour 
{
    public float circleRadiusSpeed = 5.0f;
    public float cdBeforeDeactivate = 0.5f;

    ParticleSystem mParticleSys;
    CircleCollider2D mCircleCollider;

    float mDefaultRadius, mCdTimer;
    bool mIsPlay = false;

    BombController mBombController;

	void Awake () 
    {
        mParticleSys = GetComponent<ParticleSystem>();
        mCircleCollider = GetComponent<CircleCollider2D>();

        mDefaultRadius = mCircleCollider.radius;
	}
	
	void Update () 
    {
        if (mIsPlay)
        {
            if (mParticleSys.isStopped)
            {
                mIsPlay = false;
                mCircleCollider.enabled = false;
                mCircleCollider.radius = mDefaultRadius;
            }
            else mCircleCollider.radius += Time.deltaTime * circleRadiusSpeed;
        }
        else if (!mIsPlay && mBombController != null && mBombController.IsUsingBomb)
        {
            mCdTimer += Time.deltaTime;
            if (mCdTimer > cdBeforeDeactivate)
            {
                mCdTimer = 0;
                mBombController.DeactivateBulletWipe();
            }
        }
	}

    public void SetOwnerBombCtrl(BombController bombCtrl) { mBombController = bombCtrl; }

    public void Activate()
    {
        if (!mIsPlay)
        {
            mIsPlay = true;
            mCircleCollider.enabled = true;
			mParticleSys.transform.position = mBombController.transform.position;
            mParticleSys.Play();
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!mIsPlay) return;

        if (other.tag == TagManager.sSingleton.enemyBulletTag)
        {
            other.gameObject.SetActive(false);
            ParticleSystem ps = EnvObjManager.sSingleton.GetDisappearingBulletPS();
            ps.transform.position = other.transform.position;
            ps.Play();
//            if (AudioManager.sSingleton != null) AudioManager.sSingleton.PlayBulletDisappearSfx();
        }
    }
}
