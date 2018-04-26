using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(PlayerBulletControl))]
public class PlayerBulletControlCI : Editor 
{
    PlayerBulletControl mSelf;

    List<bool> mIsShowBulletList = new List<bool>();

    void OnEnable () 
    {
        mSelf = (PlayerBulletControl)target;
        Initialize();
    }
	
    public override void OnInspectorGUI()
    {
        List<BulletManager.Bullet> bulletList = mSelf.bulletList;
        int count = bulletList.Count;

        for (int i = 0; i < count; i++)
        {
            mIsShowBulletList[i] = EditorGUILayout.Foldout(mIsShowBulletList[i], "Bullet " + (i + 1), true);

            if (mIsShowBulletList[i])
            {
                EditorGUI.indentLevel++;
                BulletManager.Bullet currBullet = bulletList[i];
                currBullet.state = (BulletManager.Bullet.State) EditorGUILayout.EnumPopup ("State", currBullet.state, GUILayout.Width(300));

                BulletManager.Bullet.State currState = currBullet.state;

                if (currState != BulletManager.Bullet.State.NONE)
                {
                    currBullet.prefab = (Transform) EditorGUILayout.ObjectField("Prefab", currBullet.prefab, typeof(Transform), false);

                    currBullet.damage = EditorGUILayout.IntField("Damage", currBullet.damage);
                    currBullet.speed = EditorGUILayout.FloatField("Speed", currBullet.speed);

                    currBullet.direction = EditorGUILayout.Vector2Field("Direction", currBullet.direction);
                    currBullet.spawnY_Offset = EditorGUILayout.FloatField("SpawnY_Offset", currBullet.spawnY_Offset);
                    currBullet.isPiercing = EditorGUILayout.Toggle("Is_Piercing", currBullet.isPiercing);

                    if (currState == BulletManager.Bullet.State.SINE_WAVE)
                    {
                        currBullet.frequency = EditorGUILayout.FloatField("Frequency", currBullet.frequency);
                        currBullet.magnitude = EditorGUILayout.FloatField("Magnitude", currBullet.magnitude);
                    }
                }
                EditorGUI.indentLevel--;
            }
        }

        EditorGUILayout.BeginHorizontal ();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Add", GUILayout.Width(40))) mSelf.AddBulletToList();
        else if (GUILayout.Button("Del", GUILayout.Width(40))) mSelf.DelBulletToList();
        EditorGUILayout.EndHorizontal ();
	}

    void Initialize()
    {
        for (int i = 0; i < mSelf.bulletList.Count; i++)
        {
            mIsShowBulletList.Add(true);
        }
    }
}
