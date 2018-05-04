using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalObject : MonoBehaviour 
{
    public enum Type
    {
        GIVE_VALUE = 0,
        DAMAGE_PLAYER
    }
    public Type type = Type.GIVE_VALUE;

    public enum State
    {
        FREE_FALL = 0,
        MOVE_TOWARDS_PLAYER
    }
    public State state = State.FREE_FALL;

    public float speedFreeFall = 1;
    public float speedToPlayer = 3;
    public float value;

    public bool isDestructable = false;
    public int hitPoint = 100;
    public float impactMultiplier = 1.0f;
    public float scoreMultiplier = 1.0f;

    Transform mPlayerHitBox;
    PlayerController mPlayer1Controller, mPlayer2Controller;

    void Start()
    {
        mPlayerHitBox = GameObject.FindGameObjectWithTag(TagManager.sSingleton.hitboxTag).transform;

        mPlayer1Controller = GameManager.sSingleton.player1.GetComponent<PlayerController>();

        if (GameManager.sSingleton.player2 != null)
            mPlayer2Controller = GameManager.sSingleton.player2.GetComponent<PlayerController>();
    }

	void Update () 
    {
        if (state == State.FREE_FALL)
        {
            Vector3 pos = transform.position;
            pos.y -= speedFreeFall * Time.deltaTime;
            transform.position = pos;
        }
        else if (state == State.MOVE_TOWARDS_PLAYER)
        {
            float deltaTime = 0;
            if (BombManager.sSingleton.isTimeStopBomb) deltaTime = Time.unscaledDeltaTime;
            else deltaTime = Time.deltaTime;

            float step = speedToPlayer * deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, mPlayerHitBox.position, step);
        }
	}

    public void SetPlayer(Transform playerHitBox) 
    { 
        if (state == State.MOVE_TOWARDS_PLAYER) return;

        state = State.MOVE_TOWARDS_PLAYER;
        mPlayerHitBox = playerHitBox; 
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isDestructable) return;

        string otherLayerName = LayerMask.LayerToName(other.gameObject.layer);
        if (otherLayerName == TagManager.sSingleton.playerBulletLayer)
        {
            if (other.tag == TagManager.sSingleton.player1BulletTag || other.tag == TagManager.sSingleton.player2BulletTag)
            {
                int damage = other.GetComponent<BulletMove>().GetBulletDamage;
                hitPoint -= damage;

                float deltaTime = 0;
                if (BombManager.sSingleton.isTimeStopBomb) deltaTime = Time.unscaledDeltaTime;
                else deltaTime = Time.deltaTime;

                // Move object up slightly.
                Vector3 pos = transform.position;
                pos.y += deltaTime * impactMultiplier;
                transform.position = pos;

                // Update player's score.
                if (other.tag == TagManager.sSingleton.player1BulletTag)
                {
                    mPlayer1Controller.UpdateLinkBar();
                    mPlayer1Controller.UpdateScore((int)(damage * scoreMultiplier));
                }
                else if (other.tag == TagManager.sSingleton.player2BulletTag)
                {
                    mPlayer2Controller.UpdateLinkBar();
                    mPlayer2Controller.UpdateScore((int)(damage * scoreMultiplier));
                }

                // TODO : Effect it does when contact.
                other.gameObject.SetActive(false);

                // TODO : Effect it does when it dies.
                if (hitPoint <= 0) gameObject.SetActive(false);
            }
        }
    }
}
