using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System;

public class PV : MonoBehaviour 
{
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;

    bool mIsChangingScene = false;
    LevelController mLevelController;

	void Start () 
    {
        MainMenuManager.sSingleton.EnableMainMenuObject(false);
        mLevelController = transform.GetComponentInParent<LevelController>();
	}
	
	void Update () 
    {
        if (!mIsChangingScene && ((videoPlayer.isPlaying && Input.anyKeyDown) || !videoPlayer.isPlaying))
        {
            mIsChangingScene = true;

            Action act = () => {MainMenuManager.sSingleton.EnableMainMenuObject(true);};
            StartCoroutine(mLevelController.Fading("MainMenu", act));
        }
	}
}
