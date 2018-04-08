using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagManager : MonoBehaviour 
{
    public static TagManager sSingleton;

    // Visual novel tags.
    public string dialogueBoxTag = "DialogueBox";
    public string leftCharacterTag = "LeftCharacter";
    public string rightCharacterTag = "RightCharacter";
    public string answerBoxTag = "AnswerBox";
    public string answerBoxBgTag = "AnswerBoxBg";

    // Touhou tags.
    public string gmTag = "GameManager";
//    public string player1Tag = "Player";
//    public string player2Tag = "Player2";
    public string hitboxTag = "Hitbox";
    public string player1BulletTag = "Player1Bullet";
    public string player2BulletTag = "Player2Bullet";
    public string enemyTag = "Enemy";
    public string enemyBulletTag = "EnemyBullet";
    public string attackPatternTag = "AttackPattern";

    // Tags for environmental objects.
    public string ENV_OBJ_PowerUpTag = "PowerUp";
    public string ENV_OBJ_ScorePickUpTag = "ScorePickUp";
    public string ENV_OBJ_DamagePlayerTag = "DamagePlayer";

    // Name for intial instantiated transform.
    public string player1BulletName = "Player1Bullet";
    public string player2BulletName = "Player2Bullet";
    public string enemy1BulletName = "Enemy1Bullet";

    // Name for UI.
    public string UI_HighScoreName = "HiScore_Display";
    public string UI_ScoreName = "Score_Display";
    public string UI_LifePointName = "Life_Display";
    public string UI_PowerLevelName = "Power_Display";
    public string UI_BombName = "Bomb_Display";
    public string UI_LinkBarName = "LinkBar_Display";
    public string UI_LinkBarInsideName = "LinkBarInside";
    public string UI_LinkMaxName = "Max";
    public string UI_PauseMenuName = "PauseMenu";

//    public string hazardRock = "Rock";

    // Layers
    public string playerBulletLayer = "PlayerBullet";

    // Sorting layers.
    public string sortLayerTopG = "TopGround";

    void Awake()
    {
        sSingleton = this;
//        player1Name = GameManager.sSingleton.player1.name;
//        player2Name = GameManager.sSingleton.player2.name;
//        enemy1Name =
    }
}
