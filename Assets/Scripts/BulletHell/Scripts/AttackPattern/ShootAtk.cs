using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShootAtk : AttackPattern 
{
    public enum Type
    {
        SINGLE_SHOT = 0,
        ANGLE_SHOT,
        ROTATE_SHOT
    }
    public Type type = Type.SINGLE_SHOT;

    public float initialSpacing;
    public int viewAngle = 90;
    public int segments;

    float angle = 0;

    public override void Start()
    {
        base.Start();
    }

    public bool IsCoroutine { get { return mIsCoroutine; } }

    public IEnumerator ShootAtPlayerSingleShot(Transform player, Func<Transform> getBulletTrans, Action doLast)
    {
        mIsCoroutine = true;
        while (mTimer < duration)
        {
            while (onceStartDelay > 0)
            {
				if (!BombManager.sSingleton.isTimeStopBomb)
                {
                    mTimer += Time.deltaTime;
                    onceStartDelay -= Time.deltaTime;
                }
                yield return null;
            }

            if (!BulletManager.sSingleton.IsDisableSpawnBullet)
            {
                Transform currBullet = getBulletTrans();

                Vector2 playerDir = (Vector2) (player.position - mOwner.transform.position).normalized;
                Vector2 pos = (Vector2) mOwner.position;
                pos += playerDir * initialSpacing;

                currBullet.position = new Vector3(pos.x, pos.y, 0);
                currBullet.gameObject.SetActive(true);

                BulletMove bulletMove = currBullet.GetComponent<BulletMove>();
                bulletMove.SetBulletValues(BulletManager.Bullet.State.ONE_DIRECTION, playerDir, bulletSpeed);
            }

            mTimer += shootDelay + Time.deltaTime;
            yield return new WaitForSeconds(shootDelay);
        }
        doLast();
        mIsCoroutine = false;
    }

    public IEnumerator ShootAtPlayerAngle(Transform player, Func<Transform> getBulletTrans, Action doLast)
    {
        mIsCoroutine = true;
        while (mTimer < duration)
        {
            while (onceStartDelay > 0)
            {
				if (!BombManager.sSingleton.isTimeStopBomb)
                {
                    mTimer += Time.deltaTime;
                    onceStartDelay -= Time.deltaTime;
                }
                yield return null;
            }

            if (!BulletManager.sSingleton.IsDisableSpawnBullet)
            {
                Vector2 playerDir = (Vector2)(player.position - mOwner.transform.position).normalized;

                angle = Vector2.Angle(playerDir, transform.up) * Mathf.Deg2Rad;
                if (playerDir.x < 0)
                    angle = -angle;

                float halfViewAngle = ((viewAngle * Mathf.Deg2Rad) / 2);
                float startAngle = angle - halfViewAngle;
                float endAngle = angle + halfViewAngle;

//                float x = Mathf.Sin(startAngle);
//                float y = Mathf.Cos(startAngle);
//                Vector2 target = new Vector3(transform.position.x + x, transform.position.y + y);
//                Debug.DrawLine (transform.position, target, Color.green);
//
//                x = Mathf.Sin(endAngle);
//                y = Mathf.Cos(endAngle);
//                target = new Vector3(transform.position.x + x, transform.position.y + y);
//                Debug.DrawLine (transform.position, target, Color.green);

                float inc = (viewAngle * Mathf.Deg2Rad) / segments;
                for (float i = startAngle; i < endAngle; i += inc)
                {
                    float x = Mathf.Sin(i);
                    float y = Mathf.Cos(i);
                    Vector2 target = new Vector3(mOwner.position.x + x, mOwner.position.y + y);
//                    Debug.DrawLine(transform.position, target, Color.red);

                    Transform currBullet = getBulletTrans();
                    currBullet.position = (Vector3)target;
                    currBullet.gameObject.SetActive(true);

                    Vector2 dir = (target - (Vector2)mOwner.position).normalized;

                    BulletMove bulletMove = currBullet.GetComponent<BulletMove>();
                    bulletMove.SetBulletValues(BulletManager.Bullet.State.ONE_DIRECTION, dir, bulletSpeed);
                }
            }

            mTimer += shootDelay + Time.deltaTime;
            yield return new WaitForSeconds(shootDelay);
        }
        doLast();
        mIsCoroutine = false;
    }
}
