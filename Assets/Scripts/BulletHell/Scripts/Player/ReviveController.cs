using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveController : MonoBehaviour
{
    public Transform otherPlayerSoulTrans;

    int mPlayerID, mCurrPressNum;
    bool mIsInsideCircle = false, mIsRevived = false;

    PlayerController mOtherPlayerController;
    PlayerSoul mOtherPlayerSoul;

    void Start()
    {
        mPlayerID = GetComponentInParent<PlayerController>().playerID;
        mOtherPlayerSoul = otherPlayerSoulTrans.GetComponent<PlayerSoul>();
        mOtherPlayerController = otherPlayerSoulTrans.GetComponentInParent<PlayerController>();
    }

    void Update()
    {
        if (mIsInsideCircle)
        {
            if((mPlayerID == 1 && Input.GetKeyDown(KeyCode.A)) || (mPlayerID == 2 && Input.GetKeyDown(KeyCode.Semicolon)))
            {
                mCurrPressNum++;
                if (mCurrPressNum >= GameManager.sSingleton.plyRevPressNum)
                {
                    mIsRevived = true;
                    mCurrPressNum = 0;
                    mOtherPlayerSoul.Deactivate();
                    mOtherPlayerController.ReviveSelf();
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == TagManager.sSingleton.reviveCircleTag)
        {
            // Stop timer.
            mOtherPlayerSoul.StopTimer();
            mIsInsideCircle = true;
            Debug.Log("Step In");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == TagManager.sSingleton.reviveCircleTag)
        {
            // Start timer.
            if(!mIsRevived) mOtherPlayerSoul.StartTimer();
            mIsRevived = false;
            mIsInsideCircle = false;
            Debug.Log("Step Out");
        }
    }
}
