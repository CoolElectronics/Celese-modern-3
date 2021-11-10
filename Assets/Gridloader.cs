using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Text;
using System.IO;
using System.IO.Compression;

[ExecuteInEditMode]
public class Gridloader : MonoBehaviour
{
    public Tilemap terrainmap;
    public Tilemap bgmap;
    public Tilemap hazardmap;

    public MapData md12;

    [SerializeField]
    TileBase debugtile;
    public bool convertTilesToBase64 = false;
    public bool convertTilesFromBase64 = false;
    [SerializeField]
    string levelToLoadDebugPleaseRemoveLaterSoThatIDontHaveToKeepUsingThisUnnecisarrilyLongVariable;

    [SerializeField]
    GameObject mapObjectToSerializeYesThisIsAnotherReallyLongDebugFunctionThatHopefullyWillBeRemovedSoonInALaterPatch;


    public List<TileDefinition> tiles;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (convertTilesToBase64){
            convertTilesToBase64 = false;
            MapData md = new MapData();

            foreach (Transform trans in mapObjectToSerializeYesThisIsAnotherReallyLongDebugFunctionThatHopefullyWillBeRemovedSoonInALaterPatch.transform){
                GameObject obj = trans.gameObject;
               if (obj.GetComponent<Room>()){
                   Room room = obj.GetComponent<Room>();
                   RoomObject serializableObject = new RoomObject();
                   serializableObject.position = trans.position;
                   serializableObject.lockXscroll = room.lockXscroll;
                   serializableObject.maxCamX = room.maxCamX;
                   serializableObject.maxCamY = room.maxCamY;
                   serializableObject.minCamX = room.minCamX;
                   serializableObject.minCamY = room.minCamY;
                   serializableObject.scrollX = room.scrollX;
                   serializableObject.scrollY = room.scrollY;
                   serializableObject.transitionBoxes = room.transitionBoxes;

                   serializableObject.defaultrespawn = room.defaultRespawn;
                    md.objs.Add(serializableObject);
               } 
            }

            string[,] terrain = new string[terrainmap.cellBounds.size.x,terrainmap.cellBounds.size.y];
            Debug.Log(terrainmap.cellBounds.size.x + " : "+ terrainmap.cellBounds.size.y);
            for (int x = terrainmap.cellBounds.min.x; x < terrainmap.cellBounds.max.x; x++) {
            for (int y = terrainmap.cellBounds.min.y; y < terrainmap.cellBounds.max.y; y++) {
                TileBase tile = terrainmap.GetTile(new Vector3Int(x,y,0));
                if (tile != null) {
                    md.terrainmap.Add(tile.name + "|" + terrainmap.GetTransformMatrix(new Vector3Int(x,y,0)).rotation.eulerAngles.z + "|" + x + "|" + y);
                }
            }
        } 

        string[,] hazard = new string[hazardmap.cellBounds.size.x,hazardmap.cellBounds.size.y];
            Debug.Log(hazardmap.cellBounds.size.x + " : "+ hazardmap.cellBounds.size.y);
            for (int x = hazardmap.cellBounds.min.x; x < hazardmap.cellBounds.max.x; x++) {
            for (int y = hazardmap.cellBounds.min.y; y < hazardmap.cellBounds.max.y; y++) {
                TileBase tile = hazardmap.GetTile(new Vector3Int(x,y,0));
                if (tile != null) {
                    md.hazardmap.Add(tile.name + "|" + hazardmap.GetTransformMatrix(new Vector3Int(x,y,0)).rotation.eulerAngles.z + "|" + x + "|" + y);
                }
            }
        }
        string[,] bg = new string[bgmap.cellBounds.size.x,bgmap.cellBounds.size.y];
            Debug.Log(bgmap.cellBounds.size.x + " : "+ bgmap.cellBounds.size.y);
            for (int x = bgmap.cellBounds.min.x; x < bgmap.cellBounds.max.x; x++) {
            for (int y = bgmap.cellBounds.min.y; y < bgmap.cellBounds.max.y; y++) {
                TileBase tile = bgmap.GetTile(new Vector3Int(x,y,0));
                if (tile != null) {
                    
                    md.bgmap.Add(tile.name + "|" + bgmap.GetTransformMatrix(new Vector3Int(x,y,0)).rotation.eulerAngles.z + "|" + x + "|" + y);
                }
            }
        } 

        // jesus christ this is a mess
        levelToLoadDebugPleaseRemoveLaterSoThatIDontHaveToKeepUsingThisUnnecisarrilyLongVariable = System.Convert.ToBase64String(Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(md)));
        Debug.Log(levelToLoadDebugPleaseRemoveLaterSoThatIDontHaveToKeepUsingThisUnnecisarrilyLongVariable);
        }
        if (convertTilesFromBase64){
            convertTilesFromBase64 = false;
            MapData md = new MapData();
            string converted = Encoding.UTF8.GetString(System.Convert.FromBase64String(levelToLoadDebugPleaseRemoveLaterSoThatIDontHaveToKeepUsingThisUnnecisarrilyLongVariable));
            md = Newtonsoft.Json.JsonConvert.DeserializeObject<MapData>(converted);

            foreach(string tiledata in md.terrainmap){
                string[] splitdata = tiledata.Split("|");
                string tilename = splitdata[0];
                TileBase tile = tiles.Find(def => def.name == tilename).tile;
                Vector3Int tilepos = new Vector3Int(System.Int32.Parse(splitdata[2]),System.Int32.Parse(splitdata[3]),0);
                Matrix4x4 tilematrix = Matrix4x4.Rotate(Quaternion.Euler(0,0,float.Parse(splitdata[1])));
                terrainmap.SetTile(tilepos,tile);
                terrainmap.SetTransformMatrix(tilepos,tilematrix);
            }
            foreach(string tiledata in md.hazardmap){
                string[] splitdata = tiledata.Split("|");
                string tilename = splitdata[0];
                TileBase tile = tiles.Find(def => def.name == tilename).tile;
                Vector3Int tilepos = new Vector3Int(System.Int32.Parse(splitdata[2]),System.Int32.Parse(splitdata[3]),0);
                Matrix4x4 tilematrix = Matrix4x4.Rotate(Quaternion.Euler(0,0,float.Parse(splitdata[1])));
                hazardmap.SetTile(tilepos,tile);
                hazardmap.SetTransformMatrix(tilepos,tilematrix);
            }
            foreach(string tiledata in md.bgmap){
                string[] splitdata = tiledata.Split("|");
                string tilename = splitdata[0];
                TileBase tile = tiles.Find(def => def.name == tilename).tile;
                Vector3Int tilepos = new Vector3Int(System.Int32.Parse(splitdata[2]),System.Int32.Parse(splitdata[3]),0);
                Matrix4x4 tilematrix = Matrix4x4.Rotate(Quaternion.Euler(0,0,float.Parse(splitdata[1])));
                bgmap.SetTile(tilepos,tile);
                bgmap.SetTransformMatrix(tilepos,tilematrix);
            }

        }
    }
}
