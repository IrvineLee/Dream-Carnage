using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BulletMove : MonoBehaviour 
{
    AttackPattern.Properties properties = new AttackPattern.Properties();
    List<AttackPattern.UpdateSpeed> newChangeList = new List<AttackPattern.UpdateSpeed>();

    Vector2 mCurrPos;
    int mCurrChangeIndex = 0;
    float mAngle = 0, mChangeSpeedTimer = 0;
    IEnumerator currCo;

	void Update () 
    {
        float deltaTime = 0;
        if (properties.isPlayer && BombManager.sSingleton.isTimeStopBomb) deltaTime = Time.unscaledDeltaTime;
        else deltaTime = Time.deltaTime;

        HandleSpeedChange();

        AttackPattern.Template currTemplate = properties.template;
        if (currTemplate == AttackPattern.Template.SINGLE_SHOT || currTemplate == AttackPattern.Template.ANGLE_SHOT || currTemplate == AttackPattern.Template.SHOOT_AROUND_IN_CIRCLE)
        {
            transform.Translate(properties.direction * properties.speed * deltaTime, Space.World);
        }
        else if (currTemplate == AttackPattern.Template.DOUBLE_SINE_WAVE)
        {
            mCurrPos += properties.direction * properties.speed * deltaTime;
            transform.position = (Vector3)mCurrPos + (Vector3)properties.curveAxis * Mathf.Sin (mAngle * properties.frequency) * (properties.magnitude + (mAngle * properties.magExpandMult));

            mAngle += deltaTime;
            if (mAngle >= (Mathf.PI * 2)) mAngle = 0;
        }
	}

    public void SetBaseProperties(AttackPattern.Properties properties) 
    { 
        this.properties.isPlayer = properties.isPlayer; 
        this.properties.template = properties.template; 
        this.properties.damage = properties.damage; 
        this.properties.speed = properties.speed; 
        this.properties.isMainPiercing = properties.isMainPiercing; 
        this.properties.isSecondaryPiercing = properties.isSecondaryPiercing; 
        this.properties.frequency = properties.frequency; 
        this.properties.magnitude = properties.magnitude; 
        this.properties.magExpandMult = properties.magExpandMult; 
    }

    public void SetProperties(AttackPattern.Template template, int damage, float speed)
    {
        this.properties.template = template;
        this.properties.damage = damage; 
        this.properties.speed = speed; 
    }

    public void SetDirection(Vector2 direction) { properties.direction = direction; }
    public void SetNewBulletSpeed(List<AttackPattern.UpdateSpeed> newList) { newChangeList = newList; }
    public void SetCurveAxis(Vector2 curveAxis) 
    {
        mCurrPos = (Vector2) transform.position;
        mAngle = 0;
        properties.curveAxis = curveAxis; 
    }

    public int GetBulletDamage { get { return properties.damage; } }
    public bool IsMainPiercing { get { return properties.isMainPiercing; } }
    public bool IsSecondaryPiercing { get { return properties.isSecondaryPiercing; } }

    void HandleSpeedChange()
    {
        // Change the speed of enemy bullet.
        if (mCurrChangeIndex < newChangeList.Count)
        {
            mChangeSpeedTimer += Time.deltaTime;

            AttackPattern.UpdateSpeed currUpdate = newChangeList[mCurrChangeIndex];

            if (mChangeSpeedTimer > currUpdate.changeSpeedTime)
            {
                // Stop the current coroutine before moving to a new speed.
                if(currCo != null) StopCoroutine(currCo);

                if (currUpdate.toSpeedQuickness == 0) properties.speed = currUpdate.toSpeed;
                else
                {
                    currCo = ToNewSpeedRoutine(currUpdate.toSpeed, currUpdate.toSpeedQuickness);
                    StartCoroutine(currCo);
                }
                mChangeSpeedTimer = 0;
                mCurrChangeIndex++;
            }
        }
    }

    IEnumerator ToNewSpeedRoutine(float toSpeed, float quickness)
    {
        while(properties.speed != toSpeed)
        {
            float value = Time.deltaTime * quickness;
            if (properties.speed < toSpeed)
            {
                properties.speed += value;
                if (properties.speed > toSpeed) properties.speed = toSpeed;
            }
            else
            {
                properties.speed -= value;
                if (properties.speed < toSpeed) properties.speed = toSpeed;
            }

            yield return null;
        }
    }
}
