using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShootAroundInCircleAtk : AttackPattern
{
    public bool clockwise = true;
    public float distance;
    public int segments;
    public float turningRate;
    public float increaseTR;
    public float increaseTRTime;
    public float maxTR;
    public float xOffset;
    public float yOffset;

    public List<BulletManager.ChangeBulletProp> slowDownList = new List<BulletManager.ChangeBulletProp>();

    float mAngle, mIncreaseTRTimer, mSlowDownTimer;

    RotateAround mRotateAround;

    public override void Start()
    {
        base.Start();
        if (GetComponent<RotateAround>() != null) mRotateAround = GetComponent<RotateAround>();
    }

    public bool IsCoroutine { get { return mIsCoroutine; } }

    public IEnumerator ShootAroundInCirclesRoutine(Func<Transform> getBulletTrans, Action doLast)
    {
        mIsCoroutine = true;

        while (mTimer < duration)
        {
            while (onceStartDelay > 0)
            {
                if (!GameManager.sSingleton.isTimeStopBomb)
                {
                    mTimer += Time.deltaTime;
                    onceStartDelay -= Time.deltaTime;
                }
                yield return null;
            }

            if (!BulletManager.sSingleton.IsDisableSpawnBullet)
            {
                mIncreaseTRTimer += Time.deltaTime;
                if (turningRate < maxTR && mIncreaseTRTimer >= increaseTRTime)
                {
                    turningRate += increaseTR;
                    if (turningRate > maxTR) turningRate = maxTR;
                    mIncreaseTRTimer = 0;
                }

                for (int i = 0; i < segments; i++)
                {
                    float x = Mathf.Sin (mAngle + (xOffset * Mathf.Deg2Rad)) * distance;
                    float y = Mathf.Cos (mAngle + (yOffset * Mathf.Deg2Rad)) * distance;
                    mAngle += (Mathf.PI * 2 / segments);

                    Vector2 dir = Vector2.zero;
                    if (clockwise) { dir.x += x;  dir.y += y; }
                    else { dir.x += y; dir.y += x; }

                    Transform currBullet = getBulletTrans();
                    currBullet.position = mOwner.position;
                    currBullet.gameObject.SetActive(true);

                    BulletMove bulletMove = currBullet.GetComponent<BulletMove>();
                    BulletManager.Bullet.State state = BulletManager.Bullet.State.ONE_DIRECTION;

                    if (mRotateAround != null)
                    {
                        bulletMove.AddRotateAround(mRotateAround);
                        state = BulletManager.Bullet.State.STYLE_ATK_1;
                    }

                    bulletMove.SetBulletValues(state, dir, bulletSpeed);
                    bulletMove.SetChangedBulletSpeed(slowDownList);
                }

                mAngle += (turningRate * Mathf.Deg2Rad);

                mTimer += shootDelay + Time.deltaTime;
                yield return new WaitForSeconds(shootDelay);
            }
            else yield return null;
        }
        doLast();
        mIsCoroutine = false;
    }
}