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
    public float plySoulTime = 11;
    public float plyRevPressNum = 10;

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

    public int plyBulletsTotal = 20;
    public int enemyBulletsTotal = 200;
    public int pickUpsTotal = 50;
    public int hazardsTotal = 10;

    void Awake()
    {
        if (_sSingleton != null && _sSingleton != this) Destroy(this.gameObject);
        else _sSingleton = this;
    }

	void Start () 
    {
        BulletPrefabData bulletData = BulletManager.sSingleton.bulletPrefabData;
        // Player main bullet instantiate.
        for (int i = 0; i < bulletData.plyMainBulletTransList.Count; i++)
        {
            Transform currBullet = bulletData.plyMainBulletTransList[i];
            BulletManager.sSingleton.InstantiateAndCacheBullet(currBullet, plyBulletsTotal, 0);
        }

        // Player secondary bullet instantiate.
        for (int i = 0; i < bulletData.plySecondaryBulletTransList.Count; i++)
        {
            Transform currBullet = bulletData.plySecondaryBulletTransList[i];
            BulletManager.sSingleton.InstantiateAndCacheBullet(currBullet, plyBulletsTotal, 1);
        }

        // Enemy bullet instantiate.
        for (int i = 0; i < bulletData.enemyBulletTransList.Count; i++)
        {
            Transform currBullet = bulletData.enemyBulletTransList[i];
            BulletManager.sSingleton.InstantiateAndCacheBullet(currBullet, enemyBulletsTotal, 2);
        }

        EnvObjPrefabData envObjData = EnvObjManager.sSingleton.envObjPrefabData;
        // Pickable environmental object instantiate.
        for (int i = 0; i < envObjData.pickableObjTransList.Count; i++)
        {
            Transform currPickableObj = envObjData.pickableObjTransList[i];
            EnvObjManager.sSingleton.InstantiateAndCacheEnvObj(currPickableObj, pickUpsTotal);
        }

        // Damagable environmental object instantiate.
        for (int i = 0; i < envObjData.hazardTransList.Count; i++)
        {
            Transform currHazard = envObjData.hazardTransList[i];
            EnvObjManager.sSingleton.InstantiateAndCacheEnvObj(currHazard, hazardsTotal);
        }
	}

    public int TotalNumOfPlayer()
    {
        int total = 0;
        if (player1 != null && player1.gameObject.activeSelf) total++;
        if (player2 != null && player2.gameObject.activeSelf) total++;
        return total;
    }

    public bool IsThisPlayerActive(int playerNum)
    {
        if (playerNum == 1 && player1 != null && player1.gameObject.activeSelf) return true;
        else if (playerNum == 2 && player2 != null && player2.gameObject.activeSelf) return true;
        return false;
    }
}
