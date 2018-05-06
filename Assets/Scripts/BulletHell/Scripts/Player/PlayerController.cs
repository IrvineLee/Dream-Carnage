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

    public Transform hitBoxTrans;
    public Transform soulTrans;
    public Transform spriteBoxTrans;

    public ChildRotateAround fairy;

    enum State
    {
        NORMAL = 0,
        DISABLE_CONTROL,
        DEAD
    };
    State state = State.NORMAL;

    static int sCurrPowerUpNum = 0;

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
	bool mIsSpeedSlow = false, mIsChangeAlpha = false, mIsInvinsible = false, mIsWaitOtherInput = false;

    int totalCoroutine = 2;
    List<bool> mIsCoroutineList = new List<bool>();

    SpriteRenderer sr;
    AttackPattern mAttackPattern;
    BombController mBombController;
    PlayerSoul mPlayerSoul;
	PlayerController mOtherPlayerController;

    void Start () 
    {
        if (GameManager.sSingleton.TotalNumOfPlayer() == 2)
        {
            if (playerID == 1) mOtherPlayerController = GameManager.sSingleton.player2.GetComponent<PlayerController>();
            else mOtherPlayerController = GameManager.sSingleton.player1.GetComponent<PlayerController>();
        }

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

        sr = GetComponent<SpriteRenderer>();
        mAttackPattern = GetComponent<AttackPattern>();
        mBombController = GetComponent<BombController>();

        // TODO : Delete null.
        if(soulTrans != null)
            mPlayerSoul = soulTrans.GetComponent<PlayerSoul>();

        for (int i = 0; i < totalCoroutine; i++)
        { mIsCoroutineList.Add(false); }

        life = GameManager.sSingleton.plyStartLife;
        bomb = GameManager.sSingleton.plyStartBomb;

        UIManager.sSingleton.UpdatePower(playerID, powerLevel, maxPowerLevel);
        UIManager.sSingleton.UpdateLinkBar(playerID, linkValue);
	}
	
	void Update () 
    {
        if ((Input.GetKeyDown(KeyCode.Escape) && playerID == 1) ||
            (Input.GetKeyDown(KeyCode.KeypadMinus) && playerID == 2))
        {
            if ((playerID == 1 && !UIManager.sSingleton.isPlayer2Pause) ||
               (playerID == 2 && !UIManager.sSingleton.isPlayer1Pause))
            {
                bool isPauseMenu = UIManager.sSingleton.IsPauseMenu;

                if (!isPauseMenu) UIManager.sSingleton.EnablePauseScreen(playerID);
                else UIManager.sSingleton.DisablePauseScreen();
            }
        }

        if (UIManager.sSingleton.IsPauseMenu) return;

        if (mIsInvinsible && !mIsChangeAlpha) StartCoroutine(GetDamagedAlphaChange());
        if (state == State.NORMAL)
        {
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
        else if (state == State.DEAD)
        {
            if (Input.GetKey(KeyCode.Return))
            {
                // Self-Revive
				MinusLife();
                ReviveSelf();
                mPlayerSoul.Deactivate();
            }
        }
        else if (state == State.DISABLE_CONTROL)
        {
            Vector3 pos = transform.position;
            pos.y += Time.deltaTime * GameManager.sSingleton.plyRespawnYSpd;
            transform.position = pos;

            mDisableCtrlTimer += Time.deltaTime;
            if (mDisableCtrlTimer >= GameManager.sSingleton.plyDisabledCtrlTime)
            {
                mDisableCtrlTimer = 0;
                state = State.NORMAL;
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

    public void ResetLinkBar()
    {
        linkValue = 0;
        UIManager.sSingleton.UpdateLinkBar(playerID, linkValue);
    }

    public void GetPowerUp(float val)
    {
        if (powerLevel < maxPowerLevel)
        {
            powerLevel += val;
            fairy.UpdateSprite(Mathf.FloorToInt(powerLevel));

            if (powerLevel > maxPowerLevel) powerLevel = maxPowerLevel;
            UIManager.sSingleton.UpdatePower(playerID, powerLevel, maxPowerLevel);
        }
    }

    public void GetDamaged()
    {
        if (mIsInvinsible) return;

//        Debug.Log("Die");
// TODO: Player destroyed animation..
//        Destroy(gameObject);

        state = State.DEAD;
        DropPower();

        // Reset the values to default.
        powerLevel = 0;
        moveSpeed = mDefaultMoveSpeed;
        mIsSpeedSlow = false;
        mIsInvinsible = true;
        fairy.UpdateSprite(Mathf.FloorToInt(powerLevel));

        // Disable currnet sprite and activate soul transform.
        sr.enabled = false;
        hitBoxTrans.gameObject.SetActive(false);
        spriteBoxTrans.gameObject.SetActive(false);
        mPlayerSoul.Activate();

        UIManager.sSingleton.UpdatePower(playerID, powerLevel, maxPowerLevel);
        BulletManager.sSingleton.DisableEnemyBullets(true);
    }

	public void MinusLife()
	{
		life -= 1;
		UIManager.sSingleton.UpdateLife(playerID, life);
	}

    public void ReviveSelf()
    {
        state = State.DISABLE_CONTROL;
        transform.position = mResetPos;

        sr.enabled = true;
        hitBoxTrans.gameObject.SetActive(true);
        spriteBoxTrans.gameObject.SetActive(true);
    }

    void HandleMovement()
    {
        if (BombManager.sSingleton.dualLinkState == BombManager.DualLinkState.PLAYER_INPUT || 
            BombManager.sSingleton.dualLinkState == BombManager.DualLinkState.ACTIVATE_PAUSE) return;

        if (playerID == 1)
        {
            //            // Player 1 only movement.
            //            if (Input.GetKey(KeyCode.UpArrow)) transform.Translate(Vector3.up * moveSpeed * Time.unscaledDeltaTime);
            //            if (Input.GetKey(KeyCode.LeftArrow)) transform.Translate(Vector3.left * moveSpeed * Time.unscaledDeltaTime);
            //            if (Input.GetKey(KeyCode.DownArrow)) transform.Translate(Vector3.down * moveSpeed * Time.unscaledDeltaTime);
            //            if (Input.GetKey(KeyCode.RightArrow)) transform.Translate(Vector3.right * moveSpeed * Time.unscaledDeltaTime);

            // Basic wasd movement.
            if (Input.GetKey(KeyCode.T)) transform.Translate(Vector3.up * moveSpeed * Time.unscaledDeltaTime);
            if (Input.GetKey(KeyCode.F)) transform.Translate(Vector3.left * moveSpeed * Time.unscaledDeltaTime);
            if (Input.GetKey(KeyCode.G)) transform.Translate(Vector3.down * moveSpeed * Time.unscaledDeltaTime);
            if (Input.GetKey(KeyCode.H)) transform.Translate(Vector3.right * moveSpeed * Time.unscaledDeltaTime);

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
        else if(playerID == 2)
        {
            // Basic wasd movement.
            if (Input.GetKey(KeyCode.UpArrow)) transform.Translate(Vector3.up * moveSpeed * Time.unscaledDeltaTime);
            if (Input.GetKey(KeyCode.LeftArrow)) transform.Translate(Vector3.left * moveSpeed * Time.unscaledDeltaTime);
            if (Input.GetKey(KeyCode.DownArrow)) transform.Translate(Vector3.down * moveSpeed * Time.unscaledDeltaTime);
            if (Input.GetKey(KeyCode.RightArrow)) transform.Translate(Vector3.right * moveSpeed * Time.unscaledDeltaTime);

            // Basic and slow movement control.
            if (Input.GetKey(KeyCode.Comma) && !mIsSpeedSlow)
            {
                moveSpeed *= 0.5f; 
                mIsSpeedSlow = true;
            }
            else if (Input.GetKeyUp(KeyCode.Comma))
            {
                moveSpeed = mDefaultMoveSpeed;
                mIsSpeedSlow = false;
            }
        }

        // Prevent player from moving out of screen.
        transform.position = (new Vector3 (
            Mathf.Clamp (transform.position.x, border.left, border.right),
            Mathf.Clamp (transform.position.y, border.bottom, border.top),
            transform.position.z)
        );
    }

    void HandleAttack()
    {
        if (BombManager.sSingleton.dualLinkState == BombManager.DualLinkState.NONE)
        {
            // Primary attack.
            if ((playerID == 1 && Input.GetKey(KeyCode.Z)) || (playerID == 2 && Input.GetKey(KeyCode.Period)))
            {
                if(!mIsCoroutineList[0]) StartCoroutine(DoFirstThenDelay(0, () => mAttackPattern.PrimaryWeaponShoot(), primaryShootDelay));
                if(powerLevel > 0 && !mIsCoroutineList[1]) StartCoroutine(DoFirstThenDelay(1, () => mAttackPattern.SecondaryWeaponShoot(), secondaryShootDelay));
            }
        }

        // Bomb.
		if (( (playerID == 1 && Input.GetKeyDown(KeyCode.X)) || (playerID == 2 && Input.GetKeyDown(KeyCode.Slash)) ) && 
			!mBombController.IsUsingBomb && !mIsWaitOtherInput )
        {
			if (bomb > 0) 
			{
                bomb -= 1;
                UIManager.sSingleton.UpdateBomb(playerID, bomb);

                if (GameManager.sSingleton.TotalNumOfPlayer() == 2)
                {
                    // The other player receiver during dual link ultimate.
                    if(BombManager.sSingleton.dualLinkState == BombManager.DualLinkState.PLAYER_INPUT)
                    {
                        Debug.Log("Activate pause before shooting.");
                        BombManager.sSingleton.dualLinkState = BombManager.DualLinkState.ACTIVATE_PAUSE;
                        mBombController.ActivatePotrait();
                    }

                    // If both players gauge are full, stop time for input of second player.
                    if (linkValue >= 1 && mOtherPlayerController.linkValue >= 1 && BombManager.sSingleton.dualLinkState == BombManager.DualLinkState.NONE) 
                    {
                        Debug.Log("Player started dual link bomb");
                        Time.timeScale = 0;
                        mBombController.ActivatePotrait();

                        mIsWaitOtherInput = true;
                        BombManager.sSingleton.dualLinkState = BombManager.DualLinkState.PLAYER_INPUT;
                        StartCoroutine (WaitOtherResponseSequence (BombManager.sSingleton.bombDualLinkInputDur));
                    }
                }

                if(BombManager.sSingleton.dualLinkState == BombManager.DualLinkState.NONE) mBombController.ActivateBomb();
			}
        }
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

            Transform currPowerUp = EnvObjManager.sSingleton.GetBigPowerUp();
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

        Color temp = sr.color;
        temp.a = GameManager.sSingleton.plyRespawnAlpha;
        sr.color = temp;

        yield return new WaitForSeconds(GameManager.sSingleton.plyAlphaBlinkDelay);

        temp.a = 1;
        sr.color = temp;

        yield return new WaitForSeconds(GameManager.sSingleton.plyAlphaBlinkDelay);
        mIsChangeAlpha = false;
    }

	IEnumerator WaitOtherResponseSequence (float stopDur)
	{
		float timer = 0;
		while(timer < stopDur)
		{
			// The real dual link activation happens in the bomb manager.
            if (BombManager.sSingleton.dualLinkState == BombManager.DualLinkState.ACTIVATE_PAUSE) 
			{
				mIsWaitOtherInput = false;
				yield break;
			}

			timer += Time.unscaledDeltaTime;
			yield return null;
		}

		Time.timeScale = 1;
		mIsWaitOtherInput = false;
        mBombController.ResetDualLinkVal();

        mBombController.ActivateBomb();
		Debug.Log ("Time ended.");
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (state != State.DEAD)
        {
            if (other.tag == TagManager.sSingleton.ENV_OBJ_PowerUp1Tag || other.tag == TagManager.sSingleton.ENV_OBJ_PowerUp2Tag ||
                other.tag == TagManager.sSingleton.ENV_OBJ_ScorePickUpTag)
                other.GetComponent<EnvironmentalObject>().SetPlayer(transform);
        }
    }
}
