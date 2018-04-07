using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardManager : MonoBehaviour 
{
    public static HazardManager sSingleton { get { return _sSingleton; } }
    static HazardManager _sSingleton;

    public class Hazard
    {
        public enum Type
        {
            ROCK = 0,
        }

        public Type type;
        public List<Transform> hazardList;

        public Hazard()
        {
            this.type = Type.ROCK;
            this.hazardList = new List<Transform>();
        }

        public Hazard(Type type, List<Transform> hazardList)
        {
            this.type = type;
            this.hazardList = hazardList;
        }
    }

    public List<Transform> hazardList = new List<Transform>();

    List<Transform> mRockList = new List<Transform>();
    List<Hazard> mAllHazardList = new List<Hazard>();

    float mDisableHazardimer, mDisableHazardTime = 0, mReturnDefaultSpdDur = 0;
    List<Transform> mStoppedEnvObjList = new List<Transform>();

    void Awake()
    {
        if (_sSingleton != null && _sSingleton != this) Destroy(this.gameObject);
        else _sSingleton = this;
    }

	void Start () 
    {
        for (int i = 0; i < mAllHazardList.Count; i++)
        {
            Hazard currHazard = mAllHazardList[i];

            if(currHazard.type == Hazard.Type.ROCK) mRockList = mAllHazardList[i].hazardList;
        }	
	}

    void Update()
    {
        if (mDisableHazardTime != 0)
        {
            mDisableHazardimer += Time.deltaTime;
            if (mDisableHazardimer > mDisableHazardTime)
            {
                mDisableHazardTime = 0;
                mDisableHazardimer = 0;
                EnableHazardMovement(mReturnDefaultSpdDur);
            }
        }
    }
    public List<Transform> GetRockList { get { return mRockList; } }

    public void InstantiateAndCacheHazards(int total)
    {
        // Group name.
        GameObject go = new GameObject();
        go.name = "Hazards";   

        for (int i = 0; i < hazardList.Count; i++)
        {
            List<Transform> currHazardList = new List<Transform>();
            string hazardName = hazardList[i].name;

            GameObject pickUpGO = new GameObject();
            pickUpGO.name = hazardName;
            pickUpGO.transform.SetParent(go.transform);

            for (int j = 0; j < total; j++)
            {
                Transform trans = Instantiate(hazardList[i], Vector3.zero, Quaternion.identity);
                trans.name = hazardName;
                trans.gameObject.SetActive(false);
                trans.SetParent(pickUpGO.transform);
                currHazardList.Add(trans);
            }

            Hazard.Type type = Hazard.Type.ROCK;
//            if(hazardName == TagManager.sSingleton.hazardRock)
            mAllHazardList.Add(new Hazard(type, currHazardList));
        }
    }

    public void TimeStopEffect(float stopDuration, float returnDefaultSpdDur)
    {
        DisableHazardMovement(stopDuration);
        mReturnDefaultSpdDur = returnDefaultSpdDur;
    }

    void EnableHazardMovement(float duration)
    {
        for (int i = 0; i < mStoppedEnvObjList.Count; i++)
        {
            Transform currTrans = mStoppedEnvObjList[i];
            if (currTrans.gameObject.activeSelf) currTrans.GetComponent<EnvironmentalObject>().EnableSpeed(duration);
        }

        mStoppedEnvObjList.Clear();
    }

    void DisableHazardMovement(float duration)
    {
        // Loop through all pick-ups.
        for (int i = 0; i < mAllHazardList.Count; i++)
        {
            Hazard currHazard = mAllHazardList[i];

            // Loop through all the same type of pick-ups.
            for (int j = 0; j < currHazard.hazardList.Count; j++)
            {
                Transform currTrans = currHazard.hazardList[j];

                if (currTrans.gameObject.activeSelf)
                {
                    currTrans.gameObject.GetComponent<EnvironmentalObject>().DisableSpeed();
                    mStoppedEnvObjList.Add(currTrans);
                }
            }
        }

        mDisableHazardTime = duration;
    }
}
