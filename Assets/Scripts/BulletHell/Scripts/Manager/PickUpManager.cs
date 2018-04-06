﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpManager : MonoBehaviour
{
    public static PickUpManager sSingleton { get { return _sSingleton; } }
    static PickUpManager _sSingleton;

    public class PickUp
    {
        public enum Type
        {
            POWER_UP = 0,
            SCORE
        }
        public Type type;
        public List<Transform> pickUpList;

        public PickUp()
        {
            this.type = Type.POWER_UP;
            this.pickUpList = new List<Transform>();
        }

        public PickUp(Type type, List<Transform> pickUpList)
        {
            this.type = type;
            this.pickUpList = pickUpList;
        }
    }

    public List<Transform> pickUpList = new List<Transform>();

    int mCurrScorePickUp = 0;
    float mDisablePickUpTimer, mDisablePickUpTime = 0, mReturnDefaultSpdDur = 0;

    List<Transform> mPowerUpSmallList = new List<Transform>();
    List<Transform> mPowerUpBigList = new List<Transform>();
    List<Transform> mScorePickUpList = new List<Transform>();
    List<PickUp> mAllPickUpList = new List<PickUp>();

    List<Transform> mStoppedEnvObjList = new List<Transform>();

    void Awake()
    {
        if (_sSingleton != null && _sSingleton != this) Destroy(this.gameObject);
        else _sSingleton = this;
    }

    void Start()
    {
        for (int i = 0; i < mAllPickUpList.Count; i++)
        {
            List<Transform> currPickUpList = mAllPickUpList[i].pickUpList;
            if (mAllPickUpList[i].type == PickUp.Type.POWER_UP)
            {
                EnvironmentalObject.Size size = mAllPickUpList[i].pickUpList[0].GetComponent<EnvironmentalObject>().size;
                if (size == EnvironmentalObject.Size.SMALL) mPowerUpSmallList = currPickUpList;
                else if (size == EnvironmentalObject.Size.BIG) mPowerUpBigList = currPickUpList;
            }
            else if (mAllPickUpList[i].type == PickUp.Type.SCORE) mScorePickUpList = currPickUpList;
        }
    }

    void Update()
    {
        if (mDisablePickUpTime != 0)
        {
            mDisablePickUpTimer += Time.deltaTime;
            if (mDisablePickUpTimer > mDisablePickUpTime)
            {
                mDisablePickUpTime = 0;
                mDisablePickUpTimer = 0;
                EnablePickUpMovement(mReturnDefaultSpdDur);
            }
        }
    }

    public void InstantiateAndCachePickUp(int total)
    {
        // Group name.
        GameObject go = new GameObject();
        go.name = "PickUp";   

        for (int i = 0; i < pickUpList.Count; i++)
        {
            List<Transform> currPickUpList = new List<Transform>();
            string pickUpName = pickUpList[i].name;

            GameObject pickUpGO = new GameObject();
            pickUpGO.name = pickUpName;
            pickUpGO.transform.SetParent(go.transform);

            for (int j = 0; j < total; j++)
            {
                Transform trans = Instantiate(pickUpList[i], Vector3.zero, Quaternion.identity);
                trans.name = pickUpName;
                trans.gameObject.SetActive(false);
                trans.SetParent(pickUpGO.transform);
                currPickUpList.Add(trans);
            }

            PickUp.Type currType = PickUp.Type.POWER_UP;
            if (pickUpName == TagManager.sSingleton.UI_ScoreTag) currType = PickUp.Type.SCORE;

            mAllPickUpList.Add(new PickUp(currType, currPickUpList));
        }
    }

    public List<Transform> GetSmallPowerUpList { get { return mPowerUpSmallList; } }
    public List<Transform> GetBigPowerUpList { get { return mPowerUpBigList; } }
    public List<Transform> GetScorePickUpList { get { return mScorePickUpList; } }

    public void TransformBulletsIntoPoints(List<BulletManager.Individual.TypeOfBullet> typeOfBulletList)
    {
        for (int i = 0; i < typeOfBulletList.Count; i++)
        {
            BulletManager.Individual.TypeOfBullet currTypeOfBullets = typeOfBulletList[i];
            for (int j = 0; j < currTypeOfBullets.bulletTransList.Count; j++)
            {
                Transform currBullet = currTypeOfBullets.bulletTransList[j];
                if (currBullet.gameObject.activeSelf)
                {
                    Transform currPoint = mScorePickUpList[mCurrScorePickUp];
                    currPoint.position = currBullet.position;
                    currPoint.gameObject.SetActive(true);

                    EnvironmentalObject currObj = currPoint.GetComponent<EnvironmentalObject>();
                    currObj.state = EnvironmentalObject.State.MOVE_TOWARDS_PLAYER;
                    currObj.speedToPlayer = GameManager.sSingleton.pointPU_SpeedToPly;
                    mCurrScorePickUp++;
                }
            }
        }
    }

    public void TimeStopEffect(float stopDuration, float returnDefaultSpdDur)
    {
        DisablePickUpMovement(stopDuration);
        mReturnDefaultSpdDur = returnDefaultSpdDur;
    }

    void EnablePickUpMovement(float duration)
    {
        bool isActive = false;
        for (int i = 0; i < mStoppedEnvObjList.Count; i++)
        {
            Transform currTrans = mStoppedEnvObjList[i];
            if (currTrans.gameObject.activeSelf)
            {
                isActive = true;
                currTrans.GetComponent<EnvironmentalObject>().EnableSpeed(duration);
            }
        }

        // If any stopped bullet is not active, wait for the duration and disable time stop.
//        if (!isActive) StartCoroutine(WaitThenDisableTimeStop(duration));
        mStoppedEnvObjList.Clear();
    }

    void DisablePickUpMovement(float duration)
    {
        // Loop through all pick-ups.
        for (int i = 0; i < mAllPickUpList.Count; i++)
        {
            PickUp currPickUp = mAllPickUpList[i];

            // Loop through all the same type of pick-ups.
            for (int j = 0; j < currPickUp.pickUpList.Count; j++)
            {
                Transform currTrans = currPickUp.pickUpList[j];

                if (currTrans.gameObject.activeSelf)
                {
                    currTrans.gameObject.GetComponent<EnvironmentalObject>().DisableSpeed();
                    mStoppedEnvObjList.Add(currTrans);
                }
            }
        }

        mDisablePickUpTime = duration;
    }
}
