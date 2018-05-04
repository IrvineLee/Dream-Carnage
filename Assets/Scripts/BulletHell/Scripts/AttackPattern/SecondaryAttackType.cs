using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondaryAttackType : MonoBehaviour 
{
    AttackPattern.SecondaryMoveTemplate moveTemplate;
    PlayerController mPlayerController;

	void Start () 
    {
        moveTemplate = GetComponent<AttackPattern>().secondaryMoveTemplate;
        mPlayerController = GetComponent<PlayerController>();
	}
	
	void Update () 
    {
        if (moveTemplate == AttackPattern.SecondaryMoveTemplate.CIRCLE_AROUND_PLAYER)
        {
            int powerLevel = Mathf.FloorToInt(mPlayerController.powerLevel);
            if (powerLevel > 0)
            {

            }
        }
	}

    public List<Vector3> GetPos(int numOfMissle, Vector2 offsetVec, float offsetBetwBullet)
    {
        List<Vector3> posList = new List<Vector3>();
        if (moveTemplate == AttackPattern.SecondaryMoveTemplate.FROM_BACK)
        {
            Vector2 pos = (Vector2)transform.position;
            Vector2 defaulPos = pos;

            for (int i = 0; i < numOfMissle; i++)
            {
                pos = defaulPos;
                pos.x += offsetVec.x;
                pos.y += offsetVec.y;

                float offset = offsetBetwBullet;
                if (numOfMissle == 2)
                {
                    if (i == 0) pos.x -= offset;
                    else if (i == 1) pos.x += offset;
                }
                else if (numOfMissle == 3)
                {
                    if (i == 0)
                    {
                        pos.x -= offset * 2;
                        pos.y -= offset;
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
                    else if (i == 1) pos.x -= offset;
                    else if (i == 2) pos.x += offset;
                    else if (i == 3)
                    {
                        pos.x += offset * 2 + offset;
                        pos.y -= offset;
                    }
                }

                posList.Add(pos);
            }
        }
        return posList;
    }
}
