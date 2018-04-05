using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpValue : MonoBehaviour 
{
    public enum Size
    {
        SMALL = 0,
        BIG
    };
    public Size size = Size.SMALL;
    public float value;
}
