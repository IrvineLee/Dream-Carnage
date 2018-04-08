using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SineWaveAtk : AttackPattern 
{
    public Vector2 offsetPosition;
    public float frequency;
    public float magnitude;
    public float magExpandMult;
    public float sineWaveBullets;
    public float cooldown;

    public override void Start()
    {
        base.Start();
    }

    public bool IsCoroutine 
    { 
        get { return mIsCoroutine; } 
        set { mIsCoroutine = value; }
    }

    public void SineWaveShoot(Transform target, Func<Transform> getBulletTrans, Action doLast)
    {
        StartCoroutine(SineWaveShootRoutine(target, true, () => getBulletTrans(), () => {} )); 
        StartCoroutine(SineWaveShootRoutine(target, false, () => getBulletTrans(), doLast)); 
    }

    IEnumerator SineWaveShootRoutine(Transform target, bool isStartLeft, Func<Transform> getBulletTrans, Action doLast)
    {
        mIsCoroutine = true;

        float timer = 0;

        while (mTimer < duration)
        {
            if (onceStartDelay != 0)
            {
                timer += onceStartDelay;
                yield return new WaitForSeconds(onceStartDelay);
                onceStartDelay = 0;
            }

            Vector2 dir = (Vector2) (target.position - mOwner.transform.position).normalized;

            Vector2 curveAxis = Vector2.zero;
            float xVal = 0, yVal = 0;

            if (dir.y < 0) xVal = -Mathf.Abs(dir.y);
            else xVal = Mathf.Abs(dir.y);

            if (dir.x < 0) yVal = Mathf.Abs(dir.x);
            else yVal = -Mathf.Abs(dir.x);

            if (isStartLeft) curveAxis = new Vector2(xVal, yVal);
            else curveAxis = new Vector2(-xVal, -yVal);

            for (int i = 0; i < sineWaveBullets; i++)
            {
                if (!BulletManager.sSingleton.IsDisableSpawnBullet && !GameManager.sSingleton.isTimeStopBomb)
                {
                    Transform currBullet = getBulletTrans();

                    Vector3 temp = mOwner.position;
                    temp.x += offsetPosition.x;
                    temp.y += offsetPosition.y;
                    currBullet.position = temp;
                    currBullet.gameObject.SetActive(true);

                    BulletMove bulletMove = currBullet.GetComponent<BulletMove>();
                    bulletMove.ResetSineWaveValue();
                    bulletMove.SetCurveAxis(curveAxis);
                    bulletMove.SetBulletValues(BulletManager.Bullet.State.SINE_WAVE, dir, bulletSpeed, magnitude, magExpandMult, frequency);
                }
                
                timer += shootDelay;
                yield return new WaitForSeconds(shootDelay);
            }
            yield return new WaitForSeconds(cooldown);

            timer += cooldown + Time.deltaTime;
            mTimer = timer;
        }
        doLast();
        mIsCoroutine = false;
    }
}
