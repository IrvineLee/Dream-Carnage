using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour 
{
    public static GameManager sSingleton { get { return _sSingleton; } }
    static GameManager _sSingleton;

    public Transform player1;
    public Transform player2;

    public int currStage = 1;

    public Transform powerSpawnTrans;
    public float powerSpawnOffset = 1;
    public int totalPowerDrop = 1;

    public int plyMaxLife = 8;
    public int plyStartLife = 2;
    public int plyMaxBomb = 5;
    public int plyStartBomb = 3;

    public float plyDisabledCtrlTime = 1;
    public float plyInvinsibilityTime = 2;
    public float plyRespawnAlpha = 0.8f;
    public float plyRespawnYSpd = 1;
    public float plyAlphaBlinkDelay = 0.2f;

    public float bulletDisappearSpeed = 1;
    public float enemyDisBulletTime = 1;
    public Color enemyDmgColor;
    public float enemyDmgColorDur = 0.1f;

    public float pointPU_SpeedToPly = 10;

    public int p1BulletsTotal = 20;
    public int p2BulletsTotal = 20;
    public int enemyBulletsTotal = 200;
    public int pickUpsTotal = 50;
    public int hazardsTotal = 10;

    [HideInInspector] public bool isTimeStopBomb = false;

    void Awake()
    {
        if (_sSingleton != null && _sSingleton != this) Destroy(this.gameObject);
        else _sSingleton = this;
    }

	void Start () 
    {
        BulletManager bulletMgr = BulletManager.sSingleton;

        // Player 1 and 2 bullet instantiate.
        if (player1 != null) bulletMgr.InstantiateAndCacheBullet(player1, p1BulletsTotal);
        if (player2 != null) bulletMgr.InstantiateAndCacheBullet(player2, p2BulletsTotal);

        // Enemy bullet instantiate.
        List<Transform> enemyList = EnemyManager.sSingleton.EnemyList;
        for (int i = 0; i < enemyList.Count; i++)
        {
            bulletMgr.InstantiateAndCacheBullet(enemyList[i], enemyBulletsTotal);
        }

        PickUpManager.sSingleton.InstantiateAndCachePickUp(pickUpsTotal);
        HazardManager.sSingleton.InstantiateAndCacheHazards(hazardsTotal);
	}

    public int TotalNumOfPlayer()
    {
        if (player2 != null) return 2;
        return 1;
    }
}
