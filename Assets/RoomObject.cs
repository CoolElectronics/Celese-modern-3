using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class RoomObject : LevelObject{
    public Vector2 defaultrespawn;
    public bool scrollX;
    public bool scrollY;
    public List<RoomTransitionBBox> transitionBoxes;
    public float minCamX;

    public float minCamY;

    public float maxCamX;

    public float maxCamY;

    public bool lockXscroll = false;
}