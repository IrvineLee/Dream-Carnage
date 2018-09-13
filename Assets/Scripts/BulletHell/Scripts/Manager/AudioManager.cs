using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour 
{
    public static AudioManager sSingleton { get { return _sSingleton; } }
    static AudioManager _sSingleton;

    // BgmSource 1 and 2 are the main bgm. BgmSource 3 is for pause menu.
    // SfxSource 1 and 2 is for player. SfxSource 3 is for enemy.
    public AudioSource bgmSource, bgmSource2, bgmSource3, sfxSource, sfxSource2, sfxSource3, sfxSource4, sfxSourceLoop, sfxSourceLoop2;

    // BGM value.
    public float fadeInSpeed = 0.1f;
    public float fadeOutSpeed = 0.1f;
    public float minBGM_Pitch = 0.7f;
    public float slowDownBGM_PitchSpeed = 1;

    // BGM audio.
    public AudioClip mainMenuBGM, inGameDialogueBGM, inGameS1BGM, inGameRainBGM, inGameS1BossBGM, gameOverBGM, winBGM, pauseBGM;

    // SFX audio.
    public AudioClip mainMenuMove, mainMenuAccept, mainMenuAccept2, mainMenuAccept3, mainMenuBack, startGame;

    // Battle sfx audio.
    public AudioClip rifleShot, sniperShot, handgunShot, shieldAreaFire, reflectedBullet, playerDeath, getRevived, playerDead;
    public AudioClip dualLinkLaser, dualLinkLightning, dualLinkSinglePress;
    public AudioClip powerPickUp, powerLevelUp, powerMax, skillClockTick, bulletClearSkill, onSoulRadius, coinGet;
    public AudioClip enemyShoot, enemyGetHit, enemyDestroyed, bossExplosion, endCurrAtk, timerRunningOut, bulletDisappear, rockDestroyed, flameTrap;
    public AudioClip pauseClick, pauseOut, hitHighScore, coinCount;

    // Character voice clip.
    public AudioClip c1AfterGiveRevive, c1AfterGiveRevive2, c1SelfRevive, c1SelfRevive2, c1UseSkill, c1UseSkill2, c1LinkFullCharge, c1LinkFullCharge2;
    public AudioClip c1LinkUse, c1LinkUse2, c1Death, c1Death2, c1RandComment, c1RandComment2, c1RandComment3, c1RandComment4;
    public AudioClip c2AfterGiveRevive, c2AfterGiveRevive2, c2SelfRevive, c2SelfRevive2, c2UseSkill, c2UseSkill2, c2LinkFullCharge, c2LinkFullCharge2;
    public AudioClip c2LinkUse, c2LinkUse2, c2Death, c2Death2, c2RandComment, c2RandComment2, c2RandComment3, c2RandComment4;
    public AudioClip c3AfterGiveRevive, c3AfterGiveRevive2, c3SelfRevive, c3SelfRevive2, c3UseSkill, c3UseSkill2, c3LinkFullCharge, c3LinkFullCharge2;
    public AudioClip c3LinkUse, c3LinkUse2, c3Death, c3Death2, c3RandComment, c3RandComment2, c3RandComment3, c3RandComment4;
    public AudioClip s1BossFinalAtk;

    Vector3 mCameraPos;
    float mDefaultBGMVol, mDefaultSFXVol;
    bool mIsKeepFadeIn = false, mIsKeepFadeOut = false;

    void Awake()
    {
        if (_sSingleton != null && _sSingleton != this) Destroy(this.gameObject);
        else _sSingleton = this;

        DontDestroyOnLoad(this.gameObject);
    }

    void Start ()
    {
        mDefaultBGMVol = bgmSource.volume;
        mDefaultSFXVol = sfxSource.volume;
        mCameraPos = Camera.main.transform.position;
    }

    // ----------------------------------------------------------------------------------------------------
    // -------------------------------------------- BGM  --------------------------------------------------
    // ----------------------------------------------------------------------------------------------------

    // ------------------------------------------ PLAY ----------------------------------------------------

    public void PlayMainBGM_Sources()
    {
        PlayBGM_Source();
        PlayBGM2_Source();
    }

    public void PlayBGM_Source() { bgmSource.Play(); }
    public void PlayBGM2_Source() { bgmSource2.Play(); }
//    public void PlayBGMAfter(float sec) { StartCoroutine(WaitThenDo(sec, () => { bgmSource.Play(); } )); }
    public void PlayMainMenuBGM() { bgmSource.clip = mainMenuBGM; bgmSource.Play(); }
    public void PlayInGameDialogueBGM() { if (!bgmSource.isPlaying) { bgmSource.clip = inGameDialogueBGM; bgmSource.Play(); } }
    public void PlayInGameStage1BGM() { if (!bgmSource.isPlaying) { bgmSource.clip = inGameS1BGM; bgmSource.Play(); } }
    public void PlayInGameRainBGM() { if (!bgmSource2.isPlaying) { bgmSource2.clip = inGameRainBGM; bgmSource2.Play(); } }
    public void PlayInGameStage1BossBGM() { if (!bgmSource.isPlaying) { bgmSource.clip = inGameS1BossBGM; bgmSource.Play(); } }

    public void PlayWinBGM() { bgmSource.clip = winBGM; bgmSource.Play(); }
    public void PlayGameOverBGM() { bgmSource.clip = gameOverBGM; bgmSource.Play(); }
    public void PlayPauseScreenBGM() { bgmSource3.clip = pauseBGM; bgmSource3.Play(); }

    // ------------------------------------------ STOP ----------------------------------------------------

    public void StopBGM() { bgmSource.Stop(); }
    public void StopMainBGM_Sources() { bgmSource.Stop(); bgmSource2.Stop(); }
    public void StopMainSFX_Sources() { sfxSourceLoop.Stop(); sfxSourceLoop2.Stop(); }
    public void StopPauseBGM() { bgmSource3.Stop(); }
    public void StopPauseScreenBGM() { bgmSource3.Stop(); }

    // ----------------------------------------------------------------------------------------------------
    // -------------------------------------------- SFX  --------------------------------------------------
    // ----------------------------------------------------------------------------------------------------

    // Main menu movement.
    public void PlayMainMenuMoveSfx() { sfxSource.PlayOneShot(mainMenuMove); }
    public void PlayMainMenuAcceptSfx() { sfxSource.PlayOneShot(mainMenuAccept); }
    public void PlayMainMenuAccept2Sfx() { sfxSource.PlayOneShot(mainMenuAccept2); }
    public void PlayMainMenuAccept3Sfx() { sfxSource.PlayOneShot(mainMenuAccept3); }
    public void PlayMainMenuBackSfx() { sfxSource.PlayOneShot(mainMenuBack); }
    public void PlayMainStartGameSfx() { sfxSource.PlayOneShot(startGame); }

    // Weapon attack.
    public void PlayRifleSfx() { if (!sfxSourceLoop.isPlaying) {sfxSourceLoop.clip = rifleShot; sfxSourceLoop.Play();} }
    public void PlaySniperSfx() { AudioSource.PlayClipAtPoint(sniperShot, mCameraPos, sfxSource.volume * 0.6f); }
    public void PlayHandgunSfx() { AudioSource.PlayClipAtPoint(handgunShot, mCameraPos, sfxSource.volume * 0.6f); }

    // Skill.
    public void PlayClockTickSfx() { sfxSource.clip = skillClockTick; sfxSource.Play(); }
    public void PlayBulletClearSfx() { AudioSource.PlayClipAtPoint(bulletClearSkill, mCameraPos, sfxSource.volume * 1); }
    public void PlayShieldAreaFireSfx() { AudioSource.PlayClipAtPoint(shieldAreaFire, mCameraPos, sfxSource.volume * 1); }
    public void PlayReflectedBulletSfx() { sfxSource2.clip = reflectedBullet; sfxSource2.Play(); }
    public void PlayDualLinkLaserSfx() { AudioSource.PlayClipAtPoint(dualLinkLaser, mCameraPos, sfxSource.volume * 1); }

    // Pick-up / Coin get / Etc.
    public void PlayGetRevivedSfx() { AudioSource.PlayClipAtPoint(getRevived, mCameraPos, sfxSource.volume * 1); }
    public void PlayCoinGetSfx() { sfxSource.clip = coinGet; sfxSource.Play(); }
    public void PlayPowerPickUpSfx() { AudioSource.PlayClipAtPoint(powerPickUp, mCameraPos, sfxSource.volume * 1); }
    public void PlayPowerLevelUpSfx() { AudioSource.PlayClipAtPoint(powerLevelUp, mCameraPos, sfxSource.volume * 0.5f); }
    public void PlayPowerMaxSfx() { AudioSource.PlayClipAtPoint(powerMax, mCameraPos, sfxSource.volume * 0.5f); }
    public void PlayOnSoulRadiusSfx() { if (!sfxSourceLoop2.isPlaying) { sfxSourceLoop2.clip = onSoulRadius; sfxSourceLoop2.Play(); } }
    public void PlayPlayerDeathSfx() { AudioSource.PlayClipAtPoint(playerDeath, mCameraPos, sfxSource.volume * 0.5f); }
    public void PlayPlayerDeadSfx() { AudioSource.PlayClipAtPoint(playerDead, mCameraPos, sfxSource.volume * 1); }
    public void PlayDualLinkSingleClickSfx() { AudioSource.PlayClipAtPoint(dualLinkSinglePress, mCameraPos, sfxSource.volume * 1); }
    public void PlayDualLinkLightningSfx() { if (!sfxSourceLoop2.isPlaying) sfxSourceLoop2.clip = dualLinkLightning; sfxSourceLoop2.Play(); }
    public void PlayFlameTrapSfx() { AudioSource.PlayClipAtPoint(flameTrap, mCameraPos, sfxSource.volume * 0.5f); }
    public void PlayHitHighScoreSfx() { AudioSource.PlayClipAtPoint(hitHighScore, mCameraPos, sfxSource.volume * 0.5f); }
    public void PlayCoinCountSfx() { AudioSource.PlayClipAtPoint(coinCount, mCameraPos, sfxSource.volume * 1); }

    // Enemy.
    public void PlayEnemyShootSfx() { sfxSource3.clip = enemyShoot; sfxSource3.Play();/*AudioSource.PlayClipAtPoint(enemyShoot, mCameraPos, sfxSource.volume * 1);*/ }
    public void PlayEnemyDestroyedSfx() { AudioSource.PlayClipAtPoint(enemyDestroyed, mCameraPos, sfxSource.volume * 1); }
    public void PlayEnemyCurrAtkSfx() { AudioSource.PlayClipAtPoint(endCurrAtk, mCameraPos, sfxSource.volume * 1); }
    public void PlayBossExplodeSfx() { sfxSource.PlayOneShot(bossExplosion); }
    public void PlayTimerRunningOutSfx() { AudioSource.PlayClipAtPoint(timerRunningOut, mCameraPos, sfxSource.volume * 1); }
    public void PlayRockDestroyedSfx() { AudioSource.PlayClipAtPoint(rockDestroyed, mCameraPos, sfxSource.volume * 0.4f); }
    public void PlayBulletDisappearSfx() { AudioSource.PlayClipAtPoint(bulletDisappear, mCameraPos, sfxSource.volume * 1); }
    public void PlayEnemyGetHitSfx() { sfxSource4.clip = enemyGetHit; sfxSource4.Play(); }

    // ------------------------------------------ STOP ----------------------------------------------------

    public void StopRifleSFX() { sfxSourceLoop.Stop(); }
    public void StopClockTickSfx() { sfxSource.Stop(); }
    public void StopOnSoulRadiusSfx() { sfxSourceLoop2.clip = null; sfxSourceLoop2.Stop(); }
    public void StopDualLinkLightningSfx() { sfxSourceLoop2.clip = null; sfxSourceLoop2.Stop(); }

    // ----------------------------------------- Resume ---------------------------------------------------

    public void ResumeOnSoulRadiusSFX() { sfxSourceLoop2.Play(); }

    // ------------------------------------------ Pause ---------------------------------------------------

    public void PauseClickOnSfx() { AudioSource.PlayClipAtPoint(pauseClick, mCameraPos, sfxSource.volume * 0.5f); }
    public void PauseClickOffSfx() { AudioSource.PlayClipAtPoint(pauseOut, mCameraPos, sfxSource.volume * 0.7f); }
    public void PauseDualLinkLightningSfx() { sfxSourceLoop2.Pause(); }
    public void PauseOnSoulRadiusSfx() { sfxSourceLoop2.Pause(); }

    // ----------------------------------------------------------------------------------------------------
    // ----------------------------------------- Voice Audio ----------------------------------------------
    // ----------------------------------------------------------------------------------------------------

    // Character 1 audio.
    public void PlayC1_GiveReviveAudio()
    {
        return;
        int val = UnityEngine.Random.Range(0, 2);
        if (val == 0) PlayC1_GiveReviveAudio1();
        else PlayC1_GiveRevive2Audio();  
    }
    public void PlayC1_SelfReviveAudio()
    {
        return;
        int val = UnityEngine.Random.Range(0, 2);
        if (val == 0) PlayC1_SelfReviveAudio1();
        else PlayC1_SelfRevive2Audio();  
    }
    public void PlayC1_UseSkillAudio()
    {
        return;
        int val = UnityEngine.Random.Range(0, 2);
        if (val == 0) PlayC1_UseSkillAudio1();
        else PlayC1_UseSkill2Audio();  
    }
    public void PlayC1_LinkFullChargeAudio()
    {
        return;
        int val = UnityEngine.Random.Range(0, 2);
        if (val == 0) PlayC1_LinkFullChargeAudio1();
        else PlayC1_LinkFullCharge2Audio();  
    }
    public void PlayC1_LinkUseAudio()
    {
        return;
        int val = UnityEngine.Random.Range(0, 2);
        if (val == 0) PlayC1_LinkUseAudio1();
        else PlayC1_LinkUse2Audio();  
    }
    public void PlayC1_DeathAudio()
    {
        return;
        int val = UnityEngine.Random.Range(0, 2);
        if (val == 0) PlayC1_DeathAudio1();
        else PlayC1_Death2Audio(); 
    }
    public void PlayC1_RandomCommentAudio()
    {
        int val = UnityEngine.Random.Range(0, 4);
        switch(val)
        {
            case 0: PlayC1_RandomComment1Audio(); break;
            case 1: PlayC1_RandomComment2Audio(); break;
            case 2: PlayC1_RandomComment3Audio(); break;
            case 3: PlayC1_RandomComment4Audio(); break;
            default: PlayC1_RandomComment1Audio(); break;
        }
    }

    // Character 2 audio.
    public void PlayC2_GiveReviveAudio()
    {
        return;
        int val = UnityEngine.Random.Range(0, 2);
        if (val == 0) PlayC2_GiveRevive1Audio();
        else PlayC2_GiveRevive2Audio(); 
    }
    public void PlayC2_SelfReviveAudio()
    {
        return;
        int val = UnityEngine.Random.Range(0, 2);
        if (val == 0) PlayC2_SelfRevive1Audio();
        else PlayC2_SelfRevive2Audio(); 
    }
    public void PlayC2_UseSkillAudio()
    {
        return;
        int val = UnityEngine.Random.Range(0, 2);
        if (val == 0) PlayC2_UseSkill1Audio();
        else PlayC2_UseSkill2Audio(); 
    }
    public void PlayC2_LinkFullChargeAudio()
    {
        return;
        int val = UnityEngine.Random.Range(0, 2);
        if (val == 0) PlayC2_LinkFullCharge1Audio();
        else PlayC2_LinkFullCharge2Audio(); 
    }
    public void PlayC2_LinkUseAudio()
    {
        return;
        int val = UnityEngine.Random.Range(0, 2);
        if (val == 0) PlayC2_LinkUse1Audio();
        else PlayC2_LinkUse2Audio(); 
    }
    public void PlayC2_DeathAudio()
    {
        return;
        int val = UnityEngine.Random.Range(0, 2);
        if (val == 0) PlayC2_Death1Audio();
        else PlayC2_Death2Audio(); 
    }
    public void PlayC2_RandomCommentAudio()
    {
        int val = UnityEngine.Random.Range(0, 4);
        switch(val)
        {
            case 0: PlayC2_RandomCommentAudio1(); break;
            case 1: PlayC2_RandomComment2Audio(); break;
            case 2: PlayC2_RandomComment3Audio(); break;
            case 3: PlayC2_RandomComment4Audio(); break;
            default: PlayC2_RandomCommentAudio1(); break;
        }
    }

    // Character 3 audio.
    public void PlayC3_GiveReviveAudio()
    {
        return;
        int val = UnityEngine.Random.Range(0, 2);
        if (val == 0) PlayC3_GiveRevive1Audio();
        else PlayC3_GiveRevive2Audio(); 
    }
    public void PlayC3_SelfReviveAudio()
    {
        return;
        int val = UnityEngine.Random.Range(0, 2);
        if (val == 0) PlayC3_SelfRevive1Audio();
        else PlayC3_SelfRevive2Audio(); 
    }
    public void PlayC3_UseSkillAudio()
    {
        return;
        int val = UnityEngine.Random.Range(0, 2);
        if (val == 0) PlayC3_UseSkill1Audio();
        else PlayC3_UseSkill2Audio(); 
    }
    public void PlayC3_LinkFullChargeAudio()
    {
        return;
        int val = UnityEngine.Random.Range(0, 2);
        if (val == 0) PlayC3_LinkFullCharge1Audio();
        else PlayC3_LinkFullCharge2Audio(); 
    }
    public void PlayC3_LinkUseAudio()
    {
        return;
        int val = UnityEngine.Random.Range(0, 2);
        if (val == 0) PlayC3_LinkUse1Audio();
        else PlayC3_LinkUse2Audio(); 
    }
    public void PlayC3_DeathAudio()
    {
        return;
        int val = UnityEngine.Random.Range(0, 2);
        if (val == 0) PlayC3_Death1Audio();
        else PlayC3_Death2Audio(); 
    }
    public void PlayC3_RandomCommentAudio()
    {
        int val = UnityEngine.Random.Range(0, 4);
        switch(val)
        {
            case 0: PlayC3_RandomComment1Audio(); break;
            case 1: PlayC3_RandomComment2Audio(); break;
            case 2: PlayC3_RandomComment3Audio(); break;
            case 3: PlayC3_RandomComment4Audio(); break;
            default: PlayC3_RandomComment1Audio(); break;
        }
    }

    public void PlayS1BossFinalAtk() { return; AudioSource.PlayClipAtPoint(s1BossFinalAtk, mCameraPos, sfxSource.volume * 1); }

    void PlayC1_GiveReviveAudio1() { AudioSource.PlayClipAtPoint(c1AfterGiveRevive, mCameraPos, sfxSource.volume * 1); }
    void PlayC1_GiveRevive2Audio() { AudioSource.PlayClipAtPoint(c1AfterGiveRevive2, mCameraPos, sfxSource.volume * 1); }
    void PlayC1_SelfReviveAudio1() { AudioSource.PlayClipAtPoint(c1SelfRevive, mCameraPos, sfxSource.volume * 1); }
    void PlayC1_SelfRevive2Audio() { AudioSource.PlayClipAtPoint(c1SelfRevive2, mCameraPos, sfxSource.volume * 1); }
    void PlayC1_UseSkillAudio1() { AudioSource.PlayClipAtPoint(c1UseSkill, mCameraPos, sfxSource.volume * 1); }
    void PlayC1_UseSkill2Audio() { AudioSource.PlayClipAtPoint(c1UseSkill2, mCameraPos, sfxSource.volume * 1); }
    void PlayC1_LinkFullChargeAudio1() { AudioSource.PlayClipAtPoint(c1LinkFullCharge, mCameraPos, sfxSource.volume * 1); }
    void PlayC1_LinkFullCharge2Audio() { AudioSource.PlayClipAtPoint(c1LinkFullCharge2, mCameraPos, sfxSource.volume * 1); }
    void PlayC1_LinkUseAudio1() { AudioSource.PlayClipAtPoint(c1LinkUse, mCameraPos, sfxSource.volume * 1); }
    void PlayC1_LinkUse2Audio() { AudioSource.PlayClipAtPoint(c1LinkUse2, mCameraPos, sfxSource.volume * 1); }
    void PlayC1_DeathAudio1() { AudioSource.PlayClipAtPoint(c1Death, mCameraPos, sfxSource.volume * 1); }
    void PlayC1_Death2Audio() { AudioSource.PlayClipAtPoint(c1Death2, mCameraPos, sfxSource.volume * 1); }
    void PlayC1_RandomComment1Audio() { AudioSource.PlayClipAtPoint(c1RandComment, mCameraPos, sfxSource.volume * 1); }
    void PlayC1_RandomComment2Audio() { AudioSource.PlayClipAtPoint(c1RandComment2, mCameraPos, sfxSource.volume * 1); }
    void PlayC1_RandomComment3Audio() { AudioSource.PlayClipAtPoint(c1RandComment3, mCameraPos, sfxSource.volume * 1); }
    void PlayC1_RandomComment4Audio() { AudioSource.PlayClipAtPoint(c1RandComment4, mCameraPos, sfxSource.volume * 1); }

    void PlayC2_GiveRevive1Audio() { AudioSource.PlayClipAtPoint(c2AfterGiveRevive, mCameraPos, sfxSource.volume * 1); }
    void PlayC2_GiveRevive2Audio() { AudioSource.PlayClipAtPoint(c2AfterGiveRevive2, mCameraPos, sfxSource.volume * 1); }
    void PlayC2_SelfRevive1Audio() { AudioSource.PlayClipAtPoint(c2SelfRevive, mCameraPos, sfxSource.volume * 1); }
    void PlayC2_SelfRevive2Audio() { AudioSource.PlayClipAtPoint(c2SelfRevive2, mCameraPos, sfxSource.volume * 1); }
    void PlayC2_UseSkill1Audio() { AudioSource.PlayClipAtPoint(c2UseSkill, mCameraPos, sfxSource.volume * 1); }
    void PlayC2_UseSkill2Audio() { AudioSource.PlayClipAtPoint(c2UseSkill2, mCameraPos, sfxSource.volume * 1); }
    void PlayC2_LinkFullCharge1Audio() { AudioSource.PlayClipAtPoint(c2LinkFullCharge, mCameraPos, sfxSource.volume * 1); }
    void PlayC2_LinkFullCharge2Audio() { AudioSource.PlayClipAtPoint(c2LinkFullCharge2, mCameraPos, sfxSource.volume * 1); }
    void PlayC2_LinkUse1Audio() { AudioSource.PlayClipAtPoint(c2LinkUse, mCameraPos, sfxSource.volume * 1); }
    void PlayC2_LinkUse2Audio() { AudioSource.PlayClipAtPoint(c2LinkUse2, mCameraPos, sfxSource.volume * 1); }
    void PlayC2_Death1Audio() { AudioSource.PlayClipAtPoint(c2Death, mCameraPos, sfxSource.volume * 1); }
    void PlayC2_Death2Audio() { AudioSource.PlayClipAtPoint(c2Death2, mCameraPos, sfxSource.volume * 1); }
    void PlayC2_RandomCommentAudio1() { AudioSource.PlayClipAtPoint(c2RandComment, mCameraPos, sfxSource.volume * 1); }
    void PlayC2_RandomComment2Audio() { AudioSource.PlayClipAtPoint(c2RandComment2, mCameraPos, sfxSource.volume * 1); }
    void PlayC2_RandomComment3Audio() { AudioSource.PlayClipAtPoint(c2RandComment3, mCameraPos, sfxSource.volume * 1); }
    void PlayC2_RandomComment4Audio() { AudioSource.PlayClipAtPoint(c2RandComment4, mCameraPos, sfxSource.volume * 1); }

    void PlayC3_GiveRevive1Audio() { AudioSource.PlayClipAtPoint(c3AfterGiveRevive, mCameraPos, sfxSource.volume * 1); }
    void PlayC3_GiveRevive2Audio() { AudioSource.PlayClipAtPoint(c3AfterGiveRevive2, mCameraPos, sfxSource.volume * 1); }
    void PlayC3_SelfRevive1Audio() { AudioSource.PlayClipAtPoint(c3SelfRevive, mCameraPos, sfxSource.volume * 1); }
    void PlayC3_SelfRevive2Audio() { AudioSource.PlayClipAtPoint(c3SelfRevive2, mCameraPos, sfxSource.volume * 1); }
    void PlayC3_UseSkill1Audio() { AudioSource.PlayClipAtPoint(c3UseSkill, mCameraPos, sfxSource.volume * 1); }
    void PlayC3_UseSkill2Audio() { AudioSource.PlayClipAtPoint(c3UseSkill2, mCameraPos, sfxSource.volume * 1); }
    void PlayC3_LinkFullCharge1Audio() { AudioSource.PlayClipAtPoint(c3LinkFullCharge, mCameraPos, sfxSource.volume * 1); }
    void PlayC3_LinkFullCharge2Audio() { AudioSource.PlayClipAtPoint(c3LinkFullCharge2, mCameraPos, sfxSource.volume * 1); }
    void PlayC3_LinkUse1Audio() { AudioSource.PlayClipAtPoint(c3LinkUse, mCameraPos, sfxSource.volume * 1); }
    void PlayC3_LinkUse2Audio() { AudioSource.PlayClipAtPoint(c3LinkUse2, mCameraPos, sfxSource.volume * 1); }
    void PlayC3_Death1Audio() { AudioSource.PlayClipAtPoint(c3Death, mCameraPos, sfxSource.volume * 1); }
    void PlayC3_Death2Audio() { AudioSource.PlayClipAtPoint(c3Death2, mCameraPos, sfxSource.volume * 1); }
    void PlayC3_RandomComment1Audio() { AudioSource.PlayClipAtPoint(c3RandComment, mCameraPos, sfxSource.volume * 1); }
    void PlayC3_RandomComment2Audio() { AudioSource.PlayClipAtPoint(c3RandComment2, mCameraPos, sfxSource.volume * 1); }
    void PlayC3_RandomComment3Audio() { AudioSource.PlayClipAtPoint(c3RandComment3, mCameraPos, sfxSource.volume * 1); }
    void PlayC3_RandomComment4Audio() { AudioSource.PlayClipAtPoint(c3RandComment4, mCameraPos, sfxSource.volume * 1); }

    // ------------------------------------- SAVE / SET VALUE ---------------------------------------------

    public void SaveCameraPos()
    {
        mCameraPos = Camera.main.transform.position;
    }

    public void SaveBGMVol(float val) 
    { 
        mDefaultBGMVol = val; 
        bgmSource.volume = val;
        bgmSource2.volume = val;
    }
    public void SaveSFXVol(float val) 
    { 
        mDefaultSFXVol = val; 
        sfxSource.volume = val;
        sfxSourceLoop.volume = val;
        sfxSourceLoop2.volume = val;
    }

    public void SetBGM_Volume(float val) { bgmSource.volume = val; }
    public void SetBGM2_Volume(float val) { bgmSource2.volume = val; }

    // ----------------------------------------------------------------------------------------------------

    public void FadeInMainBGM ()
    {
        StartCoroutine(FadeIn(bgmSource, fadeInSpeed));
        StartCoroutine(FadeIn(bgmSource2, fadeInSpeed));
    }

    // Stage audio.
    public void FadeInStageBGM ()
    {
        StartCoroutine(FadeIn(bgmSource, fadeInSpeed));
    }

    // Rain audio.
    public void FadeInRainBGM ()
    {
        StartCoroutine(FadeIn(bgmSource2, fadeInSpeed));
    }

    public void FadeOutBGM ()
    {
        FadeOutFunc(bgmSource, fadeOutSpeed, StopBGM);
    }

    public void FadeOutPauseBGM ()
    {
        FadeOutFunc(bgmSource2, fadeOutSpeed, StopAndResetPauseBGMVol);
    }

    public void PauseMainBGM_Sources()
    {
        PauseBGM_Source();
        PauseBGM2_Source();
    }

    public void PauseBGM_Source() { bgmSource.Pause(); }
    public void PauseBGM2_Source() { bgmSource2.Pause(); }

    public void SetMinBGM_Pitch()
    {
        StartCoroutine(SlowDownBGMToMinPitch(RevertBGMBackToNormalPitch));
    }

    public void ResetFadeIEnumerator()
    {
        mIsKeepFadeIn = false;
        mIsKeepFadeOut = false;
    }

    // ------------------------------------ PRIVATE FUNCTION ---------------------------------------------

    void FadeOutFunc(AudioSource audioSource, float speed, Action doLast)
    {
        if (!mIsKeepFadeOut) StartCoroutine(FadeOut(audioSource, fadeOutSpeed, doLast));
    }

    void StopAndResetPauseBGMVol()
    {
        StopPauseBGM();
        SetBGM2_Volume(mDefaultBGMVol);
    }

    IEnumerator WaitThenDo(float sec, Action doLast)
    {
        yield return new WaitForSeconds(sec);
        doLast();
    }

    IEnumerator SlowDownBGMToMinPitch(Func<IEnumerator> doLast)
    {
        while (bgmSource.pitch > minBGM_Pitch)
        {
            bgmSource.pitch -= 0.01f * slowDownBGM_PitchSpeed;
            if (bgmSource.pitch < minBGM_Pitch) bgmSource.pitch = minBGM_Pitch;

            yield return null;
        }

        StartCoroutine(doLast());
    }

    IEnumerator RevertBGMBackToNormalPitch()
    {
        float defaultBGM_Pitch = bgmSource.pitch;
        float diff = 1 - minBGM_Pitch;

        while (Time.timeScale < 1)
        {
            bgmSource.pitch = defaultBGM_Pitch + (diff * Time.timeScale);
            if (bgmSource.pitch > 1) bgmSource.pitch = 1;

            yield return null;
        }
        bgmSource.pitch = 1;
    }

    IEnumerator FadeIn (AudioSource audioSource, float speed)
    {
        mIsKeepFadeIn = true;
        mIsKeepFadeOut = false;

        float audioVol = audioSource.volume = 0;
        while (audioVol < 1 && mIsKeepFadeIn)
        {
            audioVol += speed;
            if (audioVol > 1) audioVol = 1;

            audioSource.volume = audioVol;
            yield return null;
        }
        mIsKeepFadeIn = false;
    }

    IEnumerator FadeOut (AudioSource audioSource, float speed, Action doLast)
    {
        mIsKeepFadeIn = false;
        mIsKeepFadeOut = true;

        float audioVol = audioSource.volume;
        while (audioVol > 0 && mIsKeepFadeOut)
        {
            audioVol -= speed;
            if (audioVol < 0) audioVol = 0;

            audioSource.volume = audioVol;
            yield return null;
        }
        doLast();
        mIsKeepFadeOut = false;
    }
}
