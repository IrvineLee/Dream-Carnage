using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNEnemyPrefabData : ScriptableObject 
{
    public List<Transform> playerTransList = new List<Transform>();
    public List<Transform> enemyBossTransList = new List<Transform>();
    public List<Transform> enemyMinionTransList = new List<Transform>();
    public List<Transform> enemyMinion1TypeList = new List<Transform>();
    public List<Transform> enemyMinion2TypeList = new List<Transform>();
    public List<Transform> enemyMinion3TypeList = new List<Transform>();

    public List<int> startStageDelay = new List<int>();
    public List<EnemyManager.EnemyInfo> s1EnemyMinionMoveList = new List<EnemyManager.EnemyInfo>();
    public List<EnemyManager.EnemyInfo> s1EnemyMinionMoveAftMdBossList = new List<EnemyManager.EnemyInfo>();
    public List<EnvObjManager.Rocks> s1RockSpawnList = new List<EnvObjManager.Rocks>();

    public bool isShowEnemyList = false;//, isShowEnemyList2 = false;
    public List<bool> isShowEnemyFoldoutList = new List<bool>();

	public void AddToList()
	{
		s1EnemyMinionMoveList.Add(new EnemyManager.EnemyInfo());
	}

    public void AddToList(int index, EnemyManager.EnemyInfo info)
    {
        EnemyManager.EnemyInfo newInfo = new EnemyManager.EnemyInfo();
        newInfo.groupIndex = info.groupIndex;
        newInfo.attackPatternTrans = info.attackPatternTrans;
        newInfo.movePattern = info.movePattern;
        newInfo.spawnPosition = info.spawnPosition;
        newInfo.spawnTime = info.spawnTime;

        s1EnemyMinionMoveList.Insert(index, newInfo);
    }

    public void Delete(int index)
    {
        s1EnemyMinionMoveList.RemoveAt(index);
    }

    public void Sort(ref List<EnemyManager.EnemyInfo> currList)
    {
        List<EnemyManager.EnemyInfo> spawnFirstList = new List<EnemyManager.EnemyInfo>();

        while (currList.Count != 0)
        {
            EnemyManager.EnemyInfo minSpawnTimeInfo = currList[0];
            float minSpwTime = minSpawnTimeInfo.spawnTime;
            int index = 0;

            for (int i = 1; i < currList.Count; i++)
            {
                EnemyManager.EnemyInfo currInfo = currList[i];
                if (currInfo.spawnTime < minSpwTime)
                {
                    index = i;
                    minSpwTime = currInfo.spawnTime;
                    minSpawnTimeInfo = currInfo;
                }
            }

            currList.RemoveAt(index);
            spawnFirstList.Add(minSpawnTimeInfo);
        }

        currList = spawnFirstList;
    }
}
