using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour 
{
    public static UIManager sSingleton { get { return _sSingleton; } }
    static UIManager _sSingleton;

    public Transform player1_UI;
    public Transform player2_UI;
    public Transform timerUI;

    class PlayerInfo
    {
        public Transform lifePointTrans = null, bombTrans = null;
        public Text powerLevel_UI, highScore_UI, score_UI;
        public Image linkBarImage;
        public int score;

        public PlayerInfo()
        {
            lifePointTrans = null;
            bombTrans = null;
            powerLevel_UI = null;
            highScore_UI = null;
            score_UI = null;
            linkBarImage = null;
            score = 0;
        }

        public PlayerInfo(Transform lifePointTrans, Transform bombTrans, Text powerLevel, Text highScore, Image linkBarImage, Text score)
        {
            this.lifePointTrans = lifePointTrans;
            this.bombTrans = bombTrans;
            this.powerLevel_UI = powerLevel;
            this.highScore_UI = highScore;
            this.score_UI = score;
            this.linkBarImage = linkBarImage;
            this.score = 0;
        }
    }
    List<PlayerInfo> playerUIList = new List<PlayerInfo>();

    Text mSecondText, mMilliSecondText;
    float mDuration = 0;

    void Awake()
    {
        if (_sSingleton != null && _sSingleton != this) Destroy(this.gameObject);
        else _sSingleton = this;
    }

	void Start () 
    {
        int totalPlayer = GameManager.sSingleton.TotalNumOfPlayer();
        for (int i = 0; i < totalPlayer; i++)
        { 
            playerUIList.Add(new PlayerInfo()); 

            Transform playerUI = player1_UI;
            if (i == 1) playerUI = player2_UI;

            InitializeUI(playerUI, i);
        }

        for (int i = 0; i < timerUI.childCount; i++)
        {
            Text textScript = timerUI.GetChild(i).GetComponent<Text>();

            if (i == 0) mSecondText = textScript;
            else if(i == 1)mMilliSecondText = textScript;
        }
        timerUI.gameObject.SetActive(false);
	}

    void Update()
    {
        if (mDuration != 0)
        {
            mDuration -= Time.deltaTime;
            if (mDuration < 0) mDuration = 0;
            UpdateBossTimer(mDuration);
        }
    }

    void InitializeUI(Transform playerUI, int index)
    {
        for (int i = 0; i < playerUI.childCount; i++)
        {
            Transform currChild = player1_UI.GetChild(i);
            if (currChild.name == TagManager.sSingleton.UI_HighScoreName) playerUIList[index].highScore_UI = currChild.GetComponent<Text>();
            else if (currChild.name == TagManager.sSingleton.UI_ScoreName) playerUIList[index].score_UI = currChild.GetComponent<Text>();
            else if (currChild.name == TagManager.sSingleton.UI_LifePointName) playerUIList[index].lifePointTrans = currChild;
            else if (currChild.name == TagManager.sSingleton.UI_BombName) playerUIList[index].bombTrans = currChild;
            else if (currChild.name == TagManager.sSingleton.UI_PowerLevelName) playerUIList[index].powerLevel_UI = currChild.GetComponent<Text>();
            else if (currChild.name == TagManager.sSingleton.UI_LinkBarName) playerUIList[index].linkBarImage = currChild.GetChild(0).GetComponent<Image>();
        }

        for (int i = 0; i < GameManager.sSingleton.plyStartLife; i++)
        { playerUIList[index].lifePointTrans.GetChild(i).gameObject.SetActive(true); }

        for (int i = 0; i < GameManager.sSingleton.plyStartBomb; i++)
        { playerUIList[index].bombTrans.GetChild(i).gameObject.SetActive(true); }
    }

    public void UpdateLife(int playerNum, int currLife)
    {
        for (int i = 0; i < GameManager.sSingleton.plyMaxLife; i++)
        {
            Transform currTrans = playerUIList[playerNum - 1].lifePointTrans.GetChild(i);

            if (currLife > 0) currTrans.gameObject.SetActive(true);
            else currTrans.gameObject.SetActive(false);

            currLife--;
        }
    }

    public void UpdatePower(int playerNum, float currPower, float maxPower)
    {
        string temp = currPower.ToString("F2") + " / " + maxPower.ToString("F2");
        playerUIList[playerNum - 1].powerLevel_UI.text = temp;
    }

    public void UpdateScore(int player, int addPoints)
    {
        PlayerInfo currPlayer = playerUIList[player - 1];
        currPlayer.score += addPoints;
        currPlayer.score_UI.text = GetScoreWithZero(currPlayer.score.ToString());
    }

    public void UpdateBomb(int player, int currBomb)
    {
        for (int i = 0; i < GameManager.sSingleton.plyMaxBomb; i++)
        {
            Transform currTrans = playerUIList[player - 1].bombTrans.GetChild(i);

            if (currBomb > 0) currTrans.gameObject.SetActive(true);
            else currTrans.gameObject.SetActive(false);

            currBomb--;
        }
    }

    public void UpdateLinkBar(int player, float linkVal)
    {
        playerUIList[player - 1].linkBarImage.fillAmount = linkVal;
    }

    public void ActivateBossTimer(float duration)
    {
        timerUI.gameObject.SetActive(true);
        mDuration = duration;
        UpdateBossTimer(mDuration);
    }

    void UpdateBossTimer(float duration)
    {
        float seconds = (int)duration;
        mSecondText.text = seconds.ToString();

        float milliSeconds = duration % 1;
        string milliSecStr = milliSeconds.ToString("F2");

        char c1 = milliSecStr[2];
        char c2 = milliSecStr[3];
        milliSecStr = c1.ToString() + c2.ToString();

        mMilliSecondText.text = milliSecStr;

        if(duration == 0) timerUI.gameObject.SetActive(false);
    }

    string GetScoreWithZero(string score)
    {
        int addZero = playerUIList[0].score_UI.text.Length - score.Length;

        for (int i = 0; i < addZero; i++)
        { score = "0" + score; }

        return score;
    }
}
