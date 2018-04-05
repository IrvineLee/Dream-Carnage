using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDown : MonoBehaviour 
{
    public enum State
    {
        FREE_FALL = 0,
        MOVE_TOWARDS_PLAYER
    }
    public State state = State.FREE_FALL;

    public float speed = 1;
    public float speedToPlayer = 3;

    Transform playerHitBox;

    void Start()
    {
        playerHitBox = GameObject.FindGameObjectWithTag(TagManager.sSingleton.hitboxTag).transform;
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
            transform.position = Vector3.MoveTowards(transform.position, playerHitBox.position, step);
        }
	}

    public void SetPlayer(Transform playerHitBox) 
    { 
        state = State.MOVE_TOWARDS_PLAYER;
        this.playerHitBox = playerHitBox; 
    }
}
