using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour 
{
    public int playerID = 1;
    public float primaryShootDelay = 0.05f;
    public float secondaryShootDelay = 0.15f;
    public float moveSpeed = 1.0f;
    public float respawnXPos = 0.3f;

    public int score = 0;
    public int life = 2;
    public int bomb = 2;
    public float powerLevel = 0;
    public float maxPowerLevel = 4;
    public float linkValue = 0;
    public float linkMultiplier = 0.5f;

    static int sCurrPowerUpNum = 0;
    static List<Transform> sPowerUpList;

    class Border
    {
        public float top, bottom, left, right;

        public Border()
        {
            this.top = 0;
            this.bottom = 0;
            this.left = 0;
            this.right = 0;
        }
    }
    static Border border = new Border();

    Vector3 mPlayerSize, mResetPos;
    float mDefaultMoveSpeed = 0, mDisableCtrlTimer = 0, mInvinsibilityTimer = 0;
    bool mIsSpeedSlow = false, mIsChangeAlpha = false, mIsDisableControl = false, mIsInvinsible = false;

    int totalCoroutine = 2;
    List<bool> mIsCoroutineList = new List<bool>();

    PlayerBulletControl mPlayerBulletControl;
    BombController mBombController;

    void Start () 
    {
        mDefaultMoveSpeed = moveSpeed;
        mPlayerSize = GetComponent<Renderer>().bounds.size;

        // Here is the definition of the boundary in world point
        float distance = (transform.position - Camera.main.transform.position).z;

        border.left = Camera.main.ViewportToWorldPoint (new Vector3 (0.18f, 0, distance)).x + (mPlayerSize.x/2);
        border.right = Camera.main.ViewportToWorldPoint (new Vector3 (0.82f, 0, distance)).x - (mPlayerSize.x/2);
        border.top = Camera.main.ViewportToWorldPoint (new Vector3 (0, 1, distance)).y - (mPlayerSize.y/2);
        border.bottom = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, distance)).y + (mPlayerSize.y/2);

        mResetPos.x = Camera.main.ViewportToWorldPoint (new Vector3 (respawnXPos, 0, distance)).x;
        mResetPos.y = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, distance)).y - (mPlayerSize.y/2);

        sPowerUpList = PickUpManager.sSingleton.GetBigPowerUpList;

        mPlayerBulletControl = GetComponent<PlayerBulletControl>();
        mBombController = GetComponent<BombController>();

        for (int i = 0; i < totalCoroutine; i++)
        { mIsCoroutineList.Add(false); }

        life = GameManager.sSingleton.plyStartLife;
        bomb = GameManager.sSingleton.plyStartBomb;
	}
	
	void Update () 
    {
        if (mIsInvinsible && !mIsChangeAlpha) StartCoroutine(GetDamagedAlphaChange());

        if (mIsDisableControl)
        {
            Vector3 pos = transform.position;
            pos.y += Time.deltaTime * GameManager.sSingleton.plyRespawnYSpd;
            transform.position = pos;

            mDisableCtrlTimer += Time.deltaTime;
            if (mDisableCtrlTimer >= GameManager.sSingleton.plyDisabledCtrlTime)
            {
                mDisableCtrlTimer = 0;
                mIsDisableControl = false;
            }
            if (mIsDisableControl) return;
        }

        if (mIsInvinsible)
        {
            mInvinsibilityTimer += Time.deltaTime;
            if (mInvinsibilityTimer >= GameManager.sSingleton.plyInvinsibilityTime)
            {
                mIsInvinsible = false;
                mInvinsibilityTimer = 0;
            }
        }

        HandleMovement();
        HandleAttack();
	}

    void HandleMovement()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool isPauseMenu = UIManager.sSingleton.IsPauseMenu;

            if (!isPauseMenu) UIManager.sSingleton.EnablePauseScreen(playerID);
            else UIManager.sSingleton.DisablePauseScreen();
        }

        if (UIManager.sSingleton.IsPauseMenu) return;

        // Basic wasd movement.
        if (Input.GetKey(KeyCode.UpArrow)) transform.Translate(Vector3.up * moveSpeed * Time.unscaledDeltaTime);
        if (Input.GetKey(KeyCode.LeftArrow)) transform.Translate(Vector3.left * moveSpeed * Time.unscaledDeltaTime);
        if (Input.GetKey(KeyCode.DownArrow)) transform.Translate(Vector3.down * moveSpeed * Time.unscaledDeltaTime);
        if (Input.GetKey(KeyCode.RightArrow)) transform.Translate(Vector3.right * moveSpeed * Time.unscaledDeltaTime);

//        if(Input.GetKey(KeyCode.Space)) CameraShake.sSingleton.ShakeCamera();

        // Prevent player from moving out of screen.
        transform.position = (new Vector3 (
            Mathf.Clamp (transform.position.x, border.left, border.right),
            Mathf.Clamp (transform.position.y, border.bottom, border.top),
            transform.position.z)
        );

        // Basic and slow movement control.
        if (Input.GetKey(KeyCode.LeftShift) && !mIsSpeedSlow)
        {
            moveSpeed *= 0.5f; 
            mIsSpeedSlow = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            moveSpeed = mDefaultMoveSpeed;
            mIsSpeedSlow = false;
        }
    }

    void HandleAttack()
    {
        if (UIManager.sSingleton.IsPauseMenu) return;

        // Primary attack.
        if (Input.GetKey(KeyCode.Z))
        {
            if(!mIsCoroutineList[0]) StartCoroutine(DoFirstThenDelay(0, () => mPlayerBulletControl.PrimaryWeaponShoot(0), primaryShootDelay));
            if(powerLevel > 0 && !mIsCoroutineList[1]) StartCoroutine(DoFirstThenDelay(1, () => mPlayerBulletControl.SecondaryWeaponShoot(1), secondaryShootDelay));
        }
        // Bomb.
        if (Input.GetKeyDown(KeyCode.X) && !mBombController.IsUsingBomb)
        {
            if (bomb > 0)
            {
                mBombController.ActivateBomb();
                bomb -= 1;
                UIManager.sSingleton.UpdateBomb(playerID, bomb);
            }
        }
    }

    public Vector3 PlayerSize { get { return this.mPlayerSize; } }
    public bool IsInvinsible { get { return this.mIsInvinsible; } }

    public void UpdateScore(int score)
    {
        this.score += score;
        UIManager.sSingleton.UpdateScore(playerID, this.score);
    }

    public void UpdateLinkBar()
    {
        // TODO : Value to be changed later.
        linkValue += 0.01f * linkMultiplier;
        if (linkValue > 1) linkValue = 1;
        UIManager.sSingleton.UpdateLinkBar(playerID, linkValue);
    }

    public void GetPowerUp(float val)
    {
        if (powerLevel < maxPowerLevel)
        {
            powerLevel += val;
            if (powerLevel > maxPowerLevel) powerLevel = maxPowerLevel;
            UIManager.sSingleton.UpdatePower(playerID, powerLevel, maxPowerLevel);
        }
    }

    public void GetDamaged()
    {
        if (mIsInvinsible) return;

        life -= 1;
        UIManager.sSingleton.UpdateLife(playerID, life);
//        Debug.Log("Die");
        // TODO: Player destroyed animation..
//        Destroy(gameObject);

        powerLevel = 0;
        transform.position = mResetPos;
        moveSpeed = mDefaultMoveSpeed;
        DropPower();
        UIManager.sSingleton.UpdatePower(playerID, powerLevel, maxPowerLevel);

        mIsSpeedSlow = false;
        mIsDisableControl = true;
        mIsInvinsible = true;
        BulletManager.sSingleton.DisableEnemyBullets(true);
    }

    // Drop power down on screen when died.
    void DropPower()
    {
        Vector3 spawnPos = GameManager.sSingleton.powerSpawnTrans.position;
        int total = GameManager.sSingleton.totalPowerDrop;

        for (int i = 0; i < total; i++)
        {
            Vector3 pos = spawnPos;
            float offset = GameManager.sSingleton.powerSpawnOffset;

            if (i < 2) pos.x -= (i + 1) * offset;
            else if (i > 2) pos.x += (i - 2) * offset;

            Transform currPowerUp = sPowerUpList[sCurrPowerUpNum];
            currPowerUp.position = pos;
            currPowerUp.gameObject.SetActive(true);
            sCurrPowerUpNum++;
        }
    }

    IEnumerator DoFirstThenDelay(int index, Action doFirst, float time)
    {
        mIsCoroutineList[index] = true;
        doFirst();
        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(time)); 
        mIsCoroutineList[index] = false;
    }

    IEnumerator GetDamagedAlphaChange()
    {
        mIsChangeAlpha = true;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color temp = sr.color;
        temp.a = GameManager.sSingleton.plyRespawnAlpha;
        sr.color = temp;

        yield return new WaitForSeconds(GameManager.sSingleton.plyAlphaBlinkDelay);

        temp.a = 1;
        sr.color = temp;

        yield return new WaitForSeconds(GameManager.sSingleton.plyAlphaBlinkDelay);
        mIsChangeAlpha = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == TagManager.sSingleton.ENV_OBJ_PowerUpTag || other.tag == TagManager.sSingleton.ENV_OBJ_ScorePickUpTag)
            other.GetComponent<EnvironmentalObject>().SetPlayer(transform);
    }
}
