using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalObject : MonoBehaviour 
{
    public enum Type
    {
        GET_VALUE = 0,
        DAMAGE_PLAYER
    }
    public Type type = Type.GET_VALUE;

    public enum State
    {
        FREE_FALL = 0,
        MOVE_TOWARDS_PLAYER
    }
    public State state = State.FREE_FALL;

    public float speed = 1;
    public float speedToPlayer = 3;

    public enum Size
    {
        SMALL = 0,
        BIG
    };
    public Size size = Size.SMALL;
    public float value;

    Transform mPlayerHitBox;
    float mSavedSpeed;

    void Start()
    {
        mPlayerHitBox = GameObject.FindGameObjectWithTag(TagManager.sSingleton.hitboxTag).transform;
    }

	void Update () 
    {
        if (state == State.FREE_FALL)
        {
            Vector3 pos = transform.position;
            pos.y -= speed * Time.deltaTime;
            transform.position = pos;
        }
        else if (state == State.MOVE_TOWARDS_PLAYER)
        {
            float step = speedToPlayer * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, mPlayerHitBox.position, step);
        }
	}

    public void SetPlayer(Transform playerHitBox) 
    { 
        if (state == State.MOVE_TOWARDS_PLAYER) return;

        state = State.MOVE_TOWARDS_PLAYER;
        mPlayerHitBox = playerHitBox; 
    }

    // Duration it takes for the bullet to return to default speed.
    public void EnableSpeed(float duration)
    {
        StartCoroutine(ReturnDefaultSpeedSequence(duration, mSavedSpeed));
    }

    public void DisableSpeed()
    {
        mSavedSpeed = speed;
        speed = 0;
    }

    IEnumerator ReturnDefaultSpeedSequence (float duration, float defaultSpeed)
    {
        float currTime = 0;

        while(currTime < duration)
        {
            speed = currTime / duration * defaultSpeed; 

            currTime += Time.deltaTime;
            if (currTime >= duration) speed = defaultSpeed;
            yield return null;
        }
    }
}
