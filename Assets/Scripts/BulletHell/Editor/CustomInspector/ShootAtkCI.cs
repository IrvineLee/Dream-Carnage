using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(ShootAtk))]
public class ShootAtkCI : Editor 
{
    ShootAtk mSelf;

    void OnEnable () 
    {
        mSelf = (ShootAtk)target;
    }

    public override void OnInspectorGUI()
    {
        mSelf.type = (ShootAtk.Type) EditorGUILayout.EnumPopup ("Type", mSelf.type);

        mSelf.duration = EditorGUILayout.FloatField("Duration", mSelf.duration);
        mSelf.isShowDuration = EditorGUILayout.Toggle("Is Show Duration", mSelf.isShowDuration);
        mSelf.bulletDamage = EditorGUILayout.FloatField("Bullet Damage", mSelf.bulletDamage);
        mSelf.bulletSpeed = EditorGUILayout.FloatField("Bullet Speed", mSelf.bulletSpeed);
        mSelf.shootDelay = EditorGUILayout.FloatField("Shoot Delay", mSelf.shootDelay);
        mSelf.onceStartDelay = EditorGUILayout.FloatField("Start Delay", mSelf.onceStartDelay);
        mSelf.charSprite = (Sprite) EditorGUILayout.ObjectField("Char Sprite", mSelf.charSprite, typeof(Sprite), true, GUILayout.Height(16));
        mSelf.spellCardSprite = (Sprite) EditorGUILayout.ObjectField("Spell Card Sprite", mSelf.spellCardSprite, typeof(Sprite), true, GUILayout.Height(16));

        if (mSelf.type == ShootAtk.Type.SINGLE_SHOT)
        {
            mSelf.initialSpacing = EditorGUILayout.FloatField("Initial Spacing", mSelf.initialSpacing);
        }
        else if (mSelf.type == ShootAtk.Type.ANGLE_SHOT)
        {
            mSelf.viewAngle = EditorGUILayout.IntField("View Angle", mSelf.viewAngle);
            mSelf.segments = EditorGUILayout.IntField("Segments", mSelf.segments);
        }
        if (GUI.changed) EditorUtility.SetDirty(target); 
    }
}
