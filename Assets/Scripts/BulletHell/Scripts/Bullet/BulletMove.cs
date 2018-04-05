using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove : MonoBehaviour 
{
    BulletManager.Bullet bullet = new BulletManager.Bullet();

    Vector2 mCurrPos;
    Vector3 mCurveAxis; 
    float mAngle = 0;

    int mCurrChangeIndex = 0;
    float mChangeSpeedTimer = 0, mSavedSpeed = 0;
    List<BulletManager.ChangeBulletProp> newChangeList = new List<BulletManager.ChangeBulletProp>();

    RotateAround mRotateAround;

	void Update () 
    {
        BulletManager.Bullet.State currState = bullet.state;

        if (mCurrChangeIndex < newChangeList.Count)
        {
            mChangeSpeedTimer += Time.deltaTime;
            if (mChangeSpeedTimer > newChangeList[mCurrChangeIndex].time)
            {
                bullet.speed = newChangeList[mCurrChangeIndex].speed;
                mChangeSpeedTimer = 0;
                mCurrChangeIndex++;
            }
        }

        if (currState == BulletManager.Bullet.State.NONE) return;
        else if (currState == BulletManager.Bullet.State.ONE_DIRECTION) transform.Translate(bullet.direction * bullet.speed * Time.deltaTime);
        else if (currState == BulletManager.Bullet.State.SINE_WAVE)
        {
            mCurrPos += bullet.direction * bullet.speed * Time.deltaTime;
            transform.position = (new Vector3(mCurrPos.x, mCurrPos.y, 0) + mCurveAxis * Mathf.Sin (mAngle * bullet.frequency) * (bullet.magnitude + (mAngle * bullet.magnitudeExpandMult)));

            mAngle+= Time.deltaTime;
            if (mAngle >= (Mathf.PI * 2)) mAngle = 0;
        }
        else if (currState == BulletManager.Bullet.State.STYLE_ATK_1)
        {
            transform.Translate(bullet.direction * bullet.speed * Time.deltaTime);

            if (bullet.speed == 0)
            {
                mRotateAround.radius = Vector3.Distance(mRotateAround.center.position, transform.position);
//                Debug.Log("AA");
            }
        }
	}

    // Set initial values when being instantiated.
    public void SetBulletValues(BulletManager.Bullet bullet)
    {
        this.bullet.state = bullet.state;
        this.bullet.direction = bullet.direction;
        this.bullet.damage = bullet.damage;
        this.bullet.speed = bullet.speed;
        this.bullet.frequency = bullet.frequency;
        this.bullet.magnitude = bullet.magnitude;
    }

    public void SetBulletValues(BulletManager.Bullet.State state, Vector2 direction, float speed)
    {
        this.bullet.state = state;
        this.bullet.direction = direction;
        this.bullet.speed = speed;
    }

    public void SetBulletValues(BulletManager.Bullet.State state, Vector2 direction, float speed, float magnitude, float frequency)
    {
        this.bullet.state = state;
        this.bullet.direction = direction;
        this.bullet.speed = speed;
        this.bullet.magnitude = magnitude;
        this.bullet.frequency = frequency;
    }

    public void SetBulletValues(BulletManager.Bullet.State state, Vector2 direction, float speed, float magnitude, float magnitudeExpandMult, float frequency)
    {
        this.bullet.state = state;
        this.bullet.direction = direction;
        this.bullet.speed = speed;
        this.bullet.magnitude = magnitude;
        this.bullet.magnitudeExpandMult = magnitudeExpandMult;
        this.bullet.frequency = frequency;
    }

    public void AddRotateAround(RotateAround rotateAround)
    { 
        if (rotateAround.center == null)
        {
            Debug.Log("Transform is null in RotateAround script");
            return;
        }

        RotateAround ro = gameObject.AddComponent<RotateAround>(); 
        ro.center = rotateAround.center;
        ro.radius = rotateAround.radius;
        ro.radiusSpeed = rotateAround.radiusSpeed;
        ro.rotationSpeed = rotateAround.rotationSpeed;
        mRotateAround = ro;
    }

    public void ResetSineWaveValue()
    {
        mCurrPos = (Vector2) transform.position;
        mAngle = 0;
    }

    // Duration it takes for the bullet to return to default speed.
    public void MoveBullet(float duration)
    {
//        bullet.speed = mSavedSpeed;
        StartCoroutine(ReturnDefaultSpeedSequence(duration, mSavedSpeed));
//        ;
    }

    public void StopBullet()
    {
        mSavedSpeed = bullet.speed;
        bullet.speed = 0;
    }

    public void SetChangedBulletSpeed(List<BulletManager.ChangeBulletProp> newList) { this.newChangeList = newList; }
    public void SetCurveAxis(Vector2 vec) { mCurveAxis = new Vector3(vec.x, vec.y, 0); }

    public int GetBulletDamage { get { return bullet.damage; } }
    public Vector2 BulletDirection
    { 
        get { return bullet.direction; } 
        set { bullet.direction = value; }
    }

    IEnumerator ReturnDefaultSpeedSequence (float duration, float defaultSpeed)
    {
        float currTime = 0;

        while(bullet.speed != defaultSpeed)
        {
            bullet.speed = currTime / duration * defaultSpeed; 

            currTime += Time.deltaTime;
            if (currTime > duration) currTime = duration;

            yield return null;
        }

        GameManager.sSingleton.isTimeStopBomb = false;
        BulletManager.sSingleton.IsDisableSpawnBullet = false;
    }
}
