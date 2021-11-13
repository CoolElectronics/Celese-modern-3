using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class MapData{


    public List<string> terrainmap;
    public List<string> hazardmap;
    public List<string> bgmap;

    public List<LevelObject> objs;
    public MapData(){
        terrainmap = new List<string>();
        bgmap = new List<string>();
        hazardmap = new List<string>();
        
    }   
}