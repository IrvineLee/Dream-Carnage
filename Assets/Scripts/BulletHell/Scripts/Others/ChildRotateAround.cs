using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildRotateAround : MonoBehaviour 
{
    public Transform center;
    public bool isClockwise = true;
    public Vector2 axis;
    public float radius = 1.0f;
    public float radiusSpeed = 0.5f;
    public float rotationSpeed = 80.0f;

    class Fairy
    {
        public Transform fairyTrans;
        public SpriteRenderer sr;

        public Fairy()
        {
            this.fairyTrans = null;
            this.sr = null;
        }

        public Fairy(Transform fairyTrans, SpriteRenderer sr)
        {
            this.fairyTrans = fairyTrans;
            this.sr = sr;
        }
    }
    List<Fairy> mFairyList = new List<Fairy>();

    int mActivatedFairies = 0;

    void Start () 
    {
        transform.position = (transform.position - center.position).normalized * radius + center.position;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform tempTrans = transform.GetChild(i);
            SpriteRenderer tempSr = tempTrans.GetComponent<SpriteRenderer>();

            Fairy newFairy = new Fairy(tempTrans, tempSr);
            mFairyList.Add(newFairy);
        }
    }

    void Update () 
    {
        for (int i = 0; i < mFairyList.Count; i++)
        {
            RotateAroundNoSelfRotate(i);
        }
    }

    public void UpdateSprite(int powerLevel)
    {
        if (mActivatedFairies == powerLevel) return;
        
        for (int i = 0; i < mFairyList.Count; i++)
        {
            if (i < powerLevel) mFairyList[i].sr.enabled = true;
            else mFairyList[i].sr.enabled = false;
        }
        mActivatedFairies = powerLevel;
    }

    public List<Vector3> GetFairiesPosition()
    {
        List<Vector3> tempList = new List<Vector3>();
        for (int i = 0; i < mFairyList.Count; i++)
        {
            tempList.Add(mFairyList[i].fairyTrans.position);
        }
        return tempList;
    }

    void RotateAroundNoSelfRotate(int index)
    {
        Vector3 pos = mFairyList[index].fairyTrans.transform.position;
        float angle = rotationSpeed * Time.deltaTime;

        float z = 0;
        if (isClockwise) z = -1;
        else z = 1;

        Vector3 newAxis = new Vector3(axis.x, axis.y, z);
        Quaternion rot = Quaternion.AngleAxis(angle, newAxis); // get the desired rotation
        Vector3 dir = pos - center.position; // find current direction relative to center
        dir = rot * dir; // rotate the direction
        mFairyList[index].fairyTrans.transform.position = center.position + dir; 
    }
}
