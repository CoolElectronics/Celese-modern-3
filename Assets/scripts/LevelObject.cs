using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class LevelObject{
    public float positionx;
    public float positiony;


    public string type;
    //IF:typeof(Battery)
    public float orginPosx;
    public float orginPosy;

    //IF:typeof(Room)
    public string name;
    public float defaultrespawnx;
    public float defaultrespawny;
    public bool scrollX;
    public bool scrollY;
    public List<RoomTransitionBBoxSerializable> transitionBoxes;
    public float minCamX;

    public float minCamY;

    public float maxCamX;

    public float maxCamY;

    public bool lockXscroll = false;

}