using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour 
{
    public static EnemyManager sSingleton { get { return _sSingleton; } }
    static EnemyManager _sSingleton;

	public bool isAppearEnemy = true;
    public bool isAppearBoss = true;
    public EnemyHealth bossEnemyHealthBar;

    [System.Serializable]
    public class EnemyInfo
    {
        public GroupIndex groupIndex;
        public EnemyType enemyType;

        public Transform attackPatternTrans;
		public EnemyMovement movePattern;
        public Transform spawnPosition;
        public float spawnTime;
    }

    public enum GroupIndex
    {
        MINION_1 = 0,
        MINION_2,
        MINION_3
    }

    public enum EnemyType
    {
        TYPE_A = 0,
        TYPE_B,
        TYPE_C
    }

	public List<float> meetBossTimeList = new List<float>();
    public bool isBossAppeared = false, isBossDead = false;

    [HideInInspector] public List<Transform> enemyList = new List<Transform>();
    [ReadOnlyAttribute] public int currMinion;

    // Instantiated enemy minions.
    List<List<Transform>> mEnemyMinion1List = new List<List<Transform>>();
    List<List<Transform>> mEnemyMinion2List = new List<List<Transform>>();
    List<List<Transform>> mEnemyMinion3List = new List<List<Transform>>();
    List<Transform> mEnemyBossList = new List<Transform>();

    // Separate the minions moveInfo from scriptable PlayerNEnemyPrefabData.
	List<EnemyInfo> mEnemyMinion1InfoList = new List<EnemyInfo>();
	List<EnemyInfo> mEnemyMinion2InfoList = new List<EnemyInfo>();
	List<EnemyInfo> mEnemyMinion3InfoList = new List<EnemyInfo>();
    List<EnemyInfo> mAllEnemyMinionInfoList = new List<EnemyInfo>();

    int mAddRemainingMinionAftAppear = 5;
    [SerializeField]float mTimer;
    List<int> mCurrMinionAppearList = new List<int>();

    // The index for instantiated prefab to appear. (Used for mEnemyMinionMoveList)
    int mMinion1Index, mMinion2Index, mMinion3Index, mBossIndex;

    // The index for next in line for instantiated prefab to appear. (Used for mAllEnemyMinionMoveInfoList)
    int mAppearIndex = 0;

    // The index for next in line to get moveData placed into instantiated prefab. (Used for mEnemyMinionMoveList)
    int mMinion1InfoListIndex, mMinion2InfoListIndex, mMinion3InfoListIndex;
    List<int> mMinion1IndexTypeList = new List<int>();
    List<int> mMinion2IndexTypeList = new List<int>();
    List<int> mMinion3IndexTypeList = new List<int>();

    // The saved index for Enemy moveData of each minion type. 
    // Ex: There might be 5 moveData for Minion_1 but with only 1 instantiated prefab for Minion_1. 
    // This var save the next index of moveData to be used by Minion_1. (Used for scriptableObj mEnemyMinionMoveInfoList)
    int mMinion1SavedInfoListIndex, mMinion2SavedInfoListIndex, mMinion3SavedInfoListIndex;

    int mBonusScore = 5000000, mMinusLifeBonusPoint = 2000000, mMinusSkillBonusPoint = 1000000;
    float mDelay = 0;
    bool mIsShowScoreBonus = true, mIsMinusLife = false, mIsMinusSkill = false;

    // Used for adding bonus score after defeating stage boss.
    PlayerController mPlayerController1, mPlayerController2;

    void Awake()
    {
        if (_sSingleton != null && _sSingleton != this) Destroy(this.gameObject);
        else _sSingleton = this;
    }

    void Start()
    {
        mPlayerController1 = GameManager.sSingleton.player1.GetComponent<PlayerController>();

        if (GameManager.sSingleton.IsThisPlayerActive(2))
            mPlayerController2 = GameManager.sSingleton.player2.GetComponent<PlayerController>();

        for (int i = 0; i < 3; i++)
        {
            mCurrMinionAppearList.Add(0);
            mCurrMinionAppearList.Add(0);
            mCurrMinionAppearList.Add(0);
        }
    }

    void Update()
    {
        if (GameManager.sSingleton.currState != GameManager.State.BATTLE) return;

        if (!UIManager.sSingleton.IsPauseGameOverMenu || BombManager.sSingleton.IsPause) 
		{
			mTimer += Time.deltaTime;
			if (isAppearBoss && !isBossAppeared)
			{
                if (mTimer > meetBossTimeList[GameManager.sSingleton.currStage - 1] && BombManager.sSingleton.dualLinkState != BombManager.DualLinkState.SHOOTING)
				{
					isBossAppeared = true;
					GameManager.sSingleton.currState = GameManager.State.DIALOGUE;
//					GameManager.sSingleton.currState = GameManager.State.BOSS_MOVE_INTO_SCREEN;

                    DisableAllEnemyOnScreen();
                    EnvObjManager.sSingleton.DisableAllDestroyableObj();
				}
			}

			if (isAppearEnemy) AppearEnemyMinion ();
		}

        if (isBossDead && !CoroutineUtil.isCoroutine)
        {
            System.Action action = (() => ShowAndAddBonusScore());
            if (mIsShowScoreBonus) StartCoroutine(CoroutineUtil.WaitFor(2.0f, action));
        }
    }

    public void DisableAllEnemyOnScreen()
    {
        // Disable remaining enemies on screen.
        int count = enemyList.Count;
        for (int i = 0; i < count; i++)
        {
            enemyList[0].GetComponent<EnemyBase>().PlayEnemyDeathPS();
            enemyList[0].gameObject.SetActive(false);
        }
    }

	public void AddAttackAndMovementToMinion(PlayerNEnemyPrefabData data)
    {
        // Put all enemy move list into a List, regardless of group type.
        for (int i = 0; i < data.s1EnemyMinionMoveList.Count; i++)
        {
            EnemyInfo currInfo = data.s1EnemyMinionMoveList[i];
			mAllEnemyMinionInfoList.Add(currInfo);
        }

        // Put earliest-latest spawn time minion into new List.
        List<EnemyInfo> spawnFirstList = new List<EnemyInfo>();
		while (mAllEnemyMinionInfoList.Count != 0)
        {
			EnemyInfo minSpawnTimeInfo = mAllEnemyMinionInfoList[0];

            // TODO: Hard-coded start stage delay. To be changed.
            if (data.startStageDelay.Count != 0) mDelay = data.startStageDelay[0];

            float minSpwTime = minSpawnTimeInfo.spawnTime + mDelay;
            int index = 0;

			for (int i = 1; i < mAllEnemyMinionInfoList.Count; i++)
            {
				EnemyInfo currInfo = mAllEnemyMinionInfoList[i];
                if (currInfo.spawnTime + mDelay < minSpwTime)
                {
                    index = i;
                    minSpwTime = currInfo.spawnTime + mDelay;
                    minSpawnTimeInfo = currInfo;
                }
            }

			mAllEnemyMinionInfoList.RemoveAt(index);
            spawnFirstList.Add(minSpawnTimeInfo);
        }

        // Put spawnFirstList back into allEnemyMinionList.
		mAllEnemyMinionInfoList = spawnFirstList;

        // Testing purposes.
//        for (int i = 0; i < mAllEnemyMinionInfoList.Count; i++)
//        {
//            Debug.Log(mAllEnemyMinionInfoList[i].spawnTime);
//        }

        // Separate spawnFirstList into their respective groups.
        for (int i = 0; i < spawnFirstList.Count; i++)
        {
            EnemyInfo currInfo = spawnFirstList[i];

            if (currInfo.groupIndex == GroupIndex.MINION_1) mEnemyMinion1InfoList.Add(currInfo);
			else if (currInfo.groupIndex == GroupIndex.MINION_2) mEnemyMinion2InfoList.Add(currInfo);
			else if (currInfo.groupIndex == GroupIndex.MINION_3) mEnemyMinion3InfoList.Add(currInfo);
        }

        // Add the starting index for Type A - Type C.
        for (int i = 0; i < 3; i++)
        {
            mMinion1IndexTypeList.Add(0);
            mMinion2IndexTypeList.Add(0);
            mMinion3IndexTypeList.Add(0);
        }

        // Set attack and movement info into respective enemy.
        AddAttackAndMovementToSameInstantiatedPrefabs(mEnemyMinion1InfoList, mEnemyMinion1List, ref mMinion1IndexTypeList, ref mMinion1InfoListIndex, ref mMinion1SavedInfoListIndex);
        AddAttackAndMovementToSameInstantiatedPrefabs(mEnemyMinion2InfoList, mEnemyMinion2List, ref mMinion2IndexTypeList, ref mMinion2InfoListIndex, ref mMinion2SavedInfoListIndex);
        AddAttackAndMovementToSameInstantiatedPrefabs(mEnemyMinion3InfoList, mEnemyMinion3List, ref mMinion3IndexTypeList, ref mMinion3InfoListIndex, ref mMinion3SavedInfoListIndex);

        for (int i = 0; i < 3; i++)
        {
            mMinion1IndexTypeList[i] = 0;
            mMinion2IndexTypeList[i] = 0;
            mMinion3IndexTypeList[i] = 0;
        }
    }

    public void InstantiateAndCacheEnemyBoss(Transform currEnemy)
    {
        Transform trans = Instantiate(currEnemy, currEnemy.position, Quaternion.identity);
        trans.name = currEnemy.name;
        trans.gameObject.SetActive(false);
        mEnemyBossList.Add(trans);
    }

    public void InstantiateAndCacheEnemy(Transform currEnemy, int total, int groupIndex, int enemyType)
    {
        // Group name.
        GameObject go = new GameObject();
        go.name = currEnemy.name;   

        List<Transform> sameEnemyList = new List<Transform>();
        for (int i = 0; i < total; i++)
        {
            Transform trans = Instantiate(currEnemy, Vector3.zero, Quaternion.identity);
            trans.name = currEnemy.name;
            trans.SetParent(go.transform);
            trans.gameObject.SetActive(false);

            sameEnemyList.Add(trans);

            // Set sort order for enemy bullets.
//            SpriteRenderer transSr = trans.GetComponent<SpriteRenderer>();
//            if (transSr != null && transSr.sortingLayerName == TagManager.sSingleton.sortLayerTopG) 
//                transSr.sortingOrder = i;
        }

        if (groupIndex == 0) mEnemyMinion1List.Add (sameEnemyList);
        else if (groupIndex == 1) mEnemyMinion2List.Add (sameEnemyList);
        else if (groupIndex == 2) mEnemyMinion3List.Add (sameEnemyList);
    }

    public Transform GetEnemyBossTrans()
    {
        Transform trans = mEnemyBossList[mBossIndex];
        if (mBossIndex + 1 > mEnemyBossList.Count - 1) mBossIndex = 0;
        else mBossIndex++;
        return trans;
    }
//
//    public Transform GetEnemyMinionTrans(GroupIndex groupIndex)
//    {
//        if (groupIndex == GroupIndex.MINION_1)
//        {
//            Transform trans = mEnemyMinion1List[mMinion1Index];
//            if (mMinion1Index + 1 > mEnemyMinion1List.Count - 1) mMinion1Index = 0;
//            else mMinion1Index++;
//            return trans;
//        }
//        else if (groupIndex == GroupIndex.MINION_2)
//        {
//            Transform trans = mEnemyMinion2List[mMinion2Index];
//            if (mMinion2Index + 1 > mEnemyMinion2List.Count - 1) mMinion2Index = 0;
//            else mMinion2Index++;
//            return trans;
//        }
//        else if (groupIndex == GroupIndex.MINION_3)
//        {
//            Transform trans = mEnemyMinion3List[mMinion3Index];
//            if (mMinion3Index + 1 > mEnemyMinion3List.Count - 1) mMinion3Index = 0;
//            else mMinion3Index++;
//            return trans;
//        }
//        return null;
//    }

    public void AddToList(Transform trans) { enemyList.Add(trans); }
    public void RemoveFromList(Transform trans) { enemyList.Remove(trans); }
    public void IsMinusLifeBonusPoint() { mIsMinusLife = true; }
    public void IsMinusSkillBonusPoint() { mIsMinusSkill = true; }

    public Vector2 GetClosestEnemyDir(Transform bullet)
    {
        if (enemyList.Count == 0) return Vector2.zero;
        
        Transform enemyTrans = enemyList[0];
        float minSqrLength = (enemyTrans.position - bullet.position).sqrMagnitude;

        for (int i = 1; i < enemyList.Count; i++)
        {
            Vector3 offset = enemyList[i].position - bullet.position;
            float sqrLen = offset.sqrMagnitude;
            if (sqrLen < minSqrLength)
            {
                enemyTrans = enemyList[i];
                minSqrLength = sqrLen;
            }
        }

        Vector2 dir = enemyTrans.position - bullet.position;
        return dir.normalized;
    }

    void ShowAndAddBonusScore()
    {
        mIsShowScoreBonus = false;

        if (mIsMinusLife) { mBonusScore -= mMinusLifeBonusPoint; mIsMinusLife = false; }
        if (mIsMinusSkill) { mBonusScore -= mMinusSkillBonusPoint; mIsMinusSkill = false; }

        UIManager.sSingleton.ShowBonusScore(mBonusScore);

        if (GameManager.sSingleton.TotalNumOfPlayer() == 2)
        {
            int dividedScore = mBonusScore / 2;
            mPlayerController1.score += dividedScore;
            mPlayerController2.score += dividedScore;
        }
        else if (GameManager.sSingleton.IsThisPlayerActive(1)) mPlayerController1.score += mBonusScore;
        else if (GameManager.sSingleton.IsThisPlayerActive(2)) mPlayerController2.score += mBonusScore;

        if (GameManager.sSingleton.IsThisPlayerActive(1)) UIManager.sSingleton.UpdateScore(1, mPlayerController1.score);
        if (GameManager.sSingleton.IsThisPlayerActive(2)) UIManager.sSingleton.UpdateScore(2, mPlayerController2.score);

        StartCoroutine(CoroutineUtil.WaitFor(5.0f, AfterBossIsDead));
    }

    void AfterBossIsDead()
    {
        isBossDead = false;
        UIManager.sSingleton.EnableGameOverScreen();
    }

	void AppearEnemyMinion()
	{
		for (int i = mAppearIndex; i < mAllEnemyMinionInfoList.Count; i++)
		{
            if (mTimer >= mAllEnemyMinionInfoList[i].spawnTime + mDelay)
			{
                currMinion++;
				mAppearIndex = i + 1;

                int index = -1, setNextIndex = 0;
                Transform currEnemy = null;
                GroupIndex groupIndex = mAllEnemyMinionInfoList[i].groupIndex;

                if (groupIndex == GroupIndex.MINION_1)
				{
                    if (mAllEnemyMinionInfoList[i].enemyType == EnemyType.TYPE_A)
                    {
                        index = mMinion1IndexTypeList[0];
                        currEnemy = mEnemyMinion1List[0][index].transform;

                        if (index + 1 > mEnemyMinion1List[0].Count - 1) mMinion1IndexTypeList[0] = 0;
                        else mMinion1IndexTypeList[0]++;

                        setNextIndex = mMinion1IndexTypeList[0];
                    }
                    else if (mAllEnemyMinionInfoList[i].enemyType == EnemyType.TYPE_B)
                    {
                        index = mMinion1IndexTypeList[1];
                        currEnemy = mEnemyMinion1List[1][index].transform;

                        if (index + 1 > mEnemyMinion1List[1].Count - 1) mMinion1IndexTypeList[1] = 0;
                        else mMinion1IndexTypeList[1]++;

                        setNextIndex = mMinion1IndexTypeList[1];
                    }
                    else if (mAllEnemyMinionInfoList[i].enemyType == EnemyType.TYPE_C)
                    {
                        index = mMinion1IndexTypeList[2];
                        currEnemy = mEnemyMinion1List[2][index].transform;

                        if (index + 1 > mEnemyMinion1List[2].Count - 1) mMinion1IndexTypeList[2] = 0;
                        else mMinion1IndexTypeList[2]++;

                        setNextIndex = mMinion1IndexTypeList[2];
                    }
				}
                else if (groupIndex == GroupIndex.MINION_2)
                {
                    if (mAllEnemyMinionInfoList[i].enemyType == EnemyType.TYPE_A)
                    {
                        index = mMinion2IndexTypeList[0];
                        currEnemy = mEnemyMinion2List[0][index].transform;

                        if (index + 1 > mEnemyMinion2List[0].Count - 1) mMinion2IndexTypeList[0] = 0;
                        else mMinion2IndexTypeList[0]++;

                        setNextIndex = mMinion2IndexTypeList[0];
                    }
                    else if (mAllEnemyMinionInfoList[i].enemyType == EnemyType.TYPE_B)
                    {
                        index = mMinion2IndexTypeList[1];
                        currEnemy = mEnemyMinion2List[1][index].transform;

                        if (index + 1 > mEnemyMinion2List[1].Count - 1) mMinion2IndexTypeList[1] = 0;
                        else mMinion2IndexTypeList[1]++;

                        setNextIndex = mMinion2IndexTypeList[1];
                    }
                    else if (mAllEnemyMinionInfoList[i].enemyType == EnemyType.TYPE_C)
                    {
                        index = mMinion2IndexTypeList[2];
                        currEnemy = mEnemyMinion2List[2][index].transform;

                        if (index + 1 > mEnemyMinion2List[2].Count - 1) mMinion2IndexTypeList[2] = 0;
                        else mMinion2IndexTypeList[2]++;

                        setNextIndex = mMinion2IndexTypeList[2];
                    }
                }
                else if (groupIndex == GroupIndex.MINION_3)
                {
                    if (mAllEnemyMinionInfoList[i].enemyType == EnemyType.TYPE_A)
                    {
                        index = mMinion3IndexTypeList[0];
                        currEnemy = mEnemyMinion3List[0][index].transform;

                        if (index + 1 > mEnemyMinion3List[0].Count - 1) mMinion3IndexTypeList[0] = 0;
                        else mMinion3IndexTypeList[0]++;

                        setNextIndex = mMinion3IndexTypeList[0];
                    }
                    else if (mAllEnemyMinionInfoList[i].enemyType == EnemyType.TYPE_B)
                    {
                        index = mMinion3IndexTypeList[1];
                        currEnemy = mEnemyMinion3List[1][index].transform;

                        if (index + 1 > mEnemyMinion3List[1].Count - 1) mMinion3IndexTypeList[1] = 0;
                        else mMinion3IndexTypeList[1]++;

                        setNextIndex = mMinion3IndexTypeList[1];
                    }
                    else if (mAllEnemyMinionInfoList[i].enemyType == EnemyType.TYPE_C)
                    {
                        index = mMinion3IndexTypeList[2];
                        currEnemy = mEnemyMinion3List[2][index].transform;

                        if (index + 1 > mEnemyMinion3List[2].Count - 1) mMinion3IndexTypeList[2] = 0;
                        else mMinion3IndexTypeList[2]++;

                        setNextIndex = mMinion3IndexTypeList[2];
                    }
                }

                currEnemy.position = mAllEnemyMinionInfoList[i].spawnPosition.position;
                currEnemy.gameObject.SetActive(true);


                if (groupIndex == GroupIndex.MINION_1)
                {
                    mCurrMinionAppearList[0]++;
                    if (mCurrMinionAppearList[0] >= mAddRemainingMinionAftAppear)
                    {
                        mCurrMinionAppearList[0] = 0;
                        AddAttackAndMovementToSameInstantiatedPrefabs(mEnemyMinion1InfoList, mEnemyMinion1List, ref mCurrMinionAppearList, ref mMinion1InfoListIndex, ref mMinion1SavedInfoListIndex);
                    }
                }
//                    SetNextMinionMovement(GroupIndex.MINION_1, setNextIndex);
//                else if (groupIndex == GroupIndex.MINION_2)
//                    SetNextMinionMovement(GroupIndex.MINION_2, setNextIndex);
//                else if (groupIndex == GroupIndex.MINION_3)
//                    SetNextMinionMovement(GroupIndex.MINION_3, setNextIndex);
			}
		}
	}

    void SetNextMinionMovement(GroupIndex groupIndex, int setIndex)
    {
        if (groupIndex == GroupIndex.MINION_1)
            AddAttackAndMovementToUnusedPrefab(mEnemyMinion1InfoList, mEnemyMinion1List, setIndex, ref mMinion1InfoListIndex, ref mMinion1SavedInfoListIndex);
        //        else if (groupIndex == GroupIndex.MINION_2) 
        //            AddAttackAndMovementToNextPrefab(mEnemyMinion2InfoList, mEnemyMinion2List, ref mMinion2IndexTypeList, ref mMinion2InfoListIndex, ref mMinion2SavedInfoListIndex);
        //        else if (groupIndex == GroupIndex.MINION_3) 
        //            AddAttackAndMovementToNextPrefab(mEnemyMinion3InfoList, mEnemyMinion3List, ref mMinion3IndexTypeList, ref mMinion3InfoListIndex, ref mMinion3SavedInfoListIndex);
    }

    // Set enemy attack and move pattern to everything in minionTransList.
    void AddAttackAndMovementToSameInstantiatedPrefabs(List<EnemyInfo> enemyInfoList, List<List<Transform>> minionTransList, ref List<int> currTypeList, ref int currMinionIndex, ref int savedIndex)
    {
        bool isLoop = true;
        while(isLoop)
        {
            isLoop = AddAttackAndMovementToNextPrefab(enemyInfoList, minionTransList, ref currTypeList, ref currMinionIndex, ref savedIndex);
        }
    }

    // Set enemy attack and move pattern.
    bool AddAttackAndMovementToNextPrefab(List<EnemyInfo> enemyInfoList, List<List<Transform>> minionTransList, ref List<int> currTypeList, ref int currMinionIndex, ref int savedIndex)
    {
        if (savedIndex + currMinionIndex > enemyInfoList.Count - 1) return false;
            
        EnemyInfo currInfo = enemyInfoList[savedIndex + currMinionIndex];
        AttackPattern[] currApArray = {};
        int index = -1;

        if (currInfo.enemyType == EnemyType.TYPE_A)
        {
            index = currTypeList[0];
            if (index >= minionTransList[0].Count) return false;

            currApArray = minionTransList[0][index].GetComponentsInChildren<AttackPattern>();
        }
        else if (currInfo.enemyType == EnemyType.TYPE_B)
        {
            index = currTypeList[1];
            if (savedIndex + currMinionIndex >= minionTransList[1].Count) return false;

            currApArray = minionTransList[1][index].GetComponentsInChildren<AttackPattern>();
        }
        else if (currInfo.enemyType == EnemyType.TYPE_C)
        {
            index = currTypeList[2];
            if (savedIndex + currMinionIndex >= minionTransList[2].Count) return false;

            currApArray = minionTransList[2][index].GetComponentsInChildren<AttackPattern>();
        }

        // Get all attack pattern component from current enemy, then destroy all leaving only 1.
        for (int i = 1; i < currApArray.Length; i++)
        {
            Destroy(currApArray[i]);
        }

        AttackPattern currAp = new AttackPattern();
        if (currInfo.enemyType == EnemyType.TYPE_A) currAp = minionTransList[0][index].GetComponentInChildren<AttackPattern>();
        else if (currInfo.enemyType == EnemyType.TYPE_B) currAp = minionTransList[1][index].GetComponentInChildren<AttackPattern>();
        else if (currInfo.enemyType == EnemyType.TYPE_C) currAp = minionTransList[2][index].GetComponentInChildren<AttackPattern>();

		// Set attack pattern.
        AttackPattern[] infoApArray = currInfo.attackPatternTrans.GetComponents<AttackPattern>();
        for (int i = 0; i < infoApArray.Length; i++)
        {
            if (i == 0)
                currAp.SetAttackPattern(infoApArray[0]);
            else
            {
                if (currInfo.enemyType == EnemyType.TYPE_A)
                    minionTransList[0][index].transform.GetChild(1).gameObject.AddComponent<AttackPattern>().SetAttackPattern(infoApArray[i]);
                else if (currInfo.enemyType == EnemyType.TYPE_B)
                    minionTransList[1][index].transform.GetChild(1).gameObject.AddComponent<AttackPattern>().SetAttackPattern(infoApArray[i]);
                else if (currInfo.enemyType == EnemyType.TYPE_C)
                    minionTransList[2][index].transform.GetChild(1).gameObject.AddComponent<AttackPattern>().SetAttackPattern(infoApArray[i]);
            }
        }

		// Set move pattern.
        if (currInfo.enemyType == EnemyType.TYPE_A)
            minionTransList[0][index].GetComponent<EnemyMovement>().SetMovement(currInfo.spawnPosition, currInfo.movePattern.movementList[0]);
        else if (currInfo.enemyType == EnemyType.TYPE_B)
            minionTransList[1][index].GetComponent<EnemyMovement>().SetMovement(currInfo.spawnPosition, currInfo.movePattern.movementList[0]);
        else if (currInfo.enemyType == EnemyType.TYPE_C)
            minionTransList[2][index].GetComponent<EnemyMovement>().SetMovement(currInfo.spawnPosition, currInfo.movePattern.movementList[0]);

        int count = 0;
        if (currInfo.enemyType == EnemyType.TYPE_A) count = minionTransList[0].Count;
        else if (currInfo.enemyType == EnemyType.TYPE_B) count = minionTransList[1].Count;
        else if (currInfo.enemyType == EnemyType.TYPE_C) count = minionTransList[2].Count;
            
        // If the currMinionIndex is over the instantiated prefab count, reset it back to 0(use back the first prefab).
        if (currMinionIndex + 1 > count)
        {
            savedIndex += currMinionIndex + 1;
            currMinionIndex = 0;
            return false;
        }
        else
        {
            if (currInfo.enemyType == EnemyType.TYPE_A) currTypeList[0]++;
            else if (currInfo.enemyType == EnemyType.TYPE_B) currTypeList[1]++;
            else if (currInfo.enemyType == EnemyType.TYPE_C) currTypeList[2]++;

            currMinionIndex++;
        }

        return true;
    }

    void AddAttackAndMovementToUnusedPrefab(List<EnemyInfo> enemyInfoList, List<List<Transform>> minionTransList, int currIndex, ref int currMinionIndex, ref int savedIndex)
    {
        EnemyInfo currInfo = enemyInfoList[savedIndex + currMinionIndex];
        AttackPattern[] currApArray = {};

        if (currInfo.enemyType == EnemyType.TYPE_A)
        {
            currApArray = minionTransList[0][currIndex].GetComponentsInChildren<AttackPattern>();
        }
        else if (currInfo.enemyType == EnemyType.TYPE_B)
        {
            currApArray = minionTransList[1][currIndex].GetComponentsInChildren<AttackPattern>();
        }
        else if (currInfo.enemyType == EnemyType.TYPE_C)
        {
            currApArray = minionTransList[2][currIndex].GetComponentsInChildren<AttackPattern>();
        }

        // Get all attack pattern component from current enemy, then destroy all leaving only 1.
        for (int i = 1; i < currApArray.Length; i++)
        {
            Destroy(currApArray[i]);
        }

        AttackPattern currAp = new AttackPattern();
        if (currInfo.enemyType == EnemyType.TYPE_A) currAp = minionTransList[0][currIndex].GetComponentInChildren<AttackPattern>();
        else if (currInfo.enemyType == EnemyType.TYPE_B) currAp = minionTransList[1][currIndex].GetComponentInChildren<AttackPattern>();
        else if (currInfo.enemyType == EnemyType.TYPE_C) currAp = minionTransList[2][currIndex].GetComponentInChildren<AttackPattern>();

        // Set attack pattern.
        AttackPattern[] infoApArray = currInfo.attackPatternTrans.GetComponents<AttackPattern>();
        for (int i = 0; i < infoApArray.Length; i++)
        {
            if (i == 0)
                currAp.SetAttackPattern(infoApArray[0]);
            else
            {
                if (currInfo.enemyType == EnemyType.TYPE_A)
                    minionTransList[0][currIndex].transform.GetChild(1).gameObject.AddComponent<AttackPattern>().SetAttackPattern(infoApArray[i]);
                else if (currInfo.enemyType == EnemyType.TYPE_B)
                    minionTransList[1][currIndex].transform.GetChild(1).gameObject.AddComponent<AttackPattern>().SetAttackPattern(infoApArray[i]);
                else if (currInfo.enemyType == EnemyType.TYPE_C)
                    minionTransList[2][currIndex].transform.GetChild(1).gameObject.AddComponent<AttackPattern>().SetAttackPattern(infoApArray[i]);
            }
        }

    }
}
