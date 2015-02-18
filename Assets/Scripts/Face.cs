
using UnityEngine;
using System.Collections;

public class Face : MonoBehaviour {

    public enum Direction
    {
        Front = 0, Back = 1, Left = 2, Right = 3, Top = 4, Bottom = 5
    }

    public Direction FaceDirection
    {
        get;
        set;
    }

}
