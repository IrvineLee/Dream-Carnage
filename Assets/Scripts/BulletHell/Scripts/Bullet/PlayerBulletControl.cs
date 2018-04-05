using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletControl : BulletController 
{
    public float secondaryBulletOffset = 0.2f;

    // Transform on screen.
    List<BulletManager.GroupOfBullet.TypeOfBullet> mBulletGroupList = new List<BulletManager.GroupOfBullet.TypeOfBullet>();
    List<int> mBulletNumList = new List<int>();

    PlayerController mPlayerController;

    void Start () 
    {
        mBulletGroupList = BulletManager.sSingleton.GetPlayer1BulletGroup;
        mPlayerController = GetComponent<PlayerController>();

        for (int i = 0; i < mBulletGroupList.Count; i++)
        { mBulletNumList.Add(0); }
    }

    public void PrimaryWeaponShoot(int bulletIndex)
    {
        Vector3 pos = transform.position;
        pos.y += (mPlayerController.PlayerSize.y / 2) + bulletList[bulletIndex].spawnY_Offset;

        Transform currBullet = mBulletGroupList[bulletIndex].bulletTransList[mBulletNumList[bulletIndex]];
        currBullet.position = pos;
        currBullet.gameObject.SetActive(true);

        mBulletNumList[bulletIndex]++;
        if (mBulletNumList[bulletIndex] >= mBulletGroupList[bulletIndex].bulletTransList.Count) mBulletNumList[bulletIndex] = 0;
    }

    public void SecondaryWeaponShoot(int bulletIndex)
    {
        Vector3 pos = transform.position;
        pos.y += (mPlayerController.PlayerSize.y / 2) + bulletList[bulletIndex].spawnY_Offset;

        int numOfMissle = Mathf.FloorToInt(mPlayerController.powerLevel);
        Vector2 defaulPos = pos;

        for (int i = 0; i < numOfMissle; i++)
        {
            float offset = secondaryBulletOffset;

            if (numOfMissle == 2)
            {
                if (i == 0) pos.x -= offset;
                else if (i == 1)
                {
                    pos.x = defaulPos.x;
                    pos.x += offset;
                }
            }
            else if (numOfMissle == 3)
            {
                if (i == 0)
                {
                    pos.x -= offset * 2;
                    pos.y -= offset;
                }
                else if (i == 1)
                {
                    pos = defaulPos;
                }
                else if (i == 2)
                {
                    pos.x += offset * 2;
                    pos.y -= offset;
                }
            }
            else if (numOfMissle == 4)
            {
                if (i == 0)
                {
                    pos.x -= offset * 2 + offset;
                    pos.y -= offset;
                }
                else if (i == 1)
                {
                    pos = defaulPos;
                    pos.x -= offset;
                }
                else if (i == 2)
                {
                    pos = defaulPos;
                    pos.x += offset;
                }
                else if (i == 3)
                {
                    pos = defaulPos;
                    pos.x += offset * 2 + offset;
                    pos.y -= offset;
                }
            }

            Transform currBullet = mBulletGroupList[bulletIndex].bulletTransList[mBulletNumList[bulletIndex]];
            currBullet.position = pos;
            currBullet.gameObject.GetComponent<BulletMove>().BulletDirection = bulletList[1].direction;
            currBullet.gameObject.SetActive(true);

            mBulletNumList[bulletIndex]++;
            if (mBulletNumList[bulletIndex] >= mBulletGroupList[bulletIndex].bulletTransList.Count) mBulletNumList[bulletIndex] = 0;
        }
    }
}
