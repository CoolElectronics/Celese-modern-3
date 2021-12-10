using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using System;
using System.Text;
using System.IO;
using System.IO.Compression;
using TMPro;
[ExecuteInEditMode]
public class Gridloader : MonoBehaviour
{
    public Tilemap terrainmap;
    public Tilemap bgmap;
    public Tilemap hazardmap;

    public MapData md12;

    [SerializeField]
    TileBase debugtile;


    [SerializeField]
    GameObject mapObjectToSerializeYesThisIsAnotherReallyLongDebugFunctionThatHopefullyWillBeRemovedSoonInALaterPatch;

    [SerializeField]
    GameObject batteryPrefab;
    [SerializeField]
    GameObject roomPrefab;
    [SerializeField]
    GameObject levelContainerPrefab;
    [SerializeField]
    LayerMask roomLmPlayer;

    [SerializeField]
    GameObject editorContainer;

    public List<TileDefinition> tiles;
    public static Gridloader i;

    public bool saveLevelDebug = false;

    public bool rasterRuleTilesInSelection = false;
    void Awake()
    {
        i = this;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (saveLevelDebug)
        {
            saveLevelDebug = false;
            SaveLevel();
        }
    }

    // jesus christ this is a mess

    public void LoadLevel(LevelHash hash)
    {
        MapData md = new MapData();
        string converted = Encoding.UTF8.GetString(System.Convert.FromBase64String(hash.leveldata));
        md = JsonUtility.FromJson<MapData>(converted);
        GameObject GeneratedLevelContainer;
        if (!hash.isLevelEditor)
        {
            GeneratedLevelContainer = Instantiate(levelContainerPrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            GeneratedLevelContainer = editorContainer;
            terrainmap.BoxFill(new Vector3Int(0, 0, 0), null, terrainmap.cellBounds.min.x, terrainmap.cellBounds.min.y, terrainmap.cellBounds.max.x, terrainmap.cellBounds.max.y);
        }
        if (hash.officialLevel)
        {
            if (hash.extraPrefab != "")
            {
                Debug.Log("Attempting to load extra prefabs");
                GameObject extralvl = (GameObject)Resources.Load(hash.extraPrefab);
                if (extralvl)
                {
                    Instantiate(extralvl, GeneratedLevelContainer.transform);
                }
            }
        }
        foreach (LevelObject obj in md.objs)
        {
            Debug.Log("atttempting to construct LevelObject " + obj);
            if (obj.type == "Battery")
            {
                Debug.Log("Constructing BatteryObject");
                GameObject instantiated = Instantiate(batteryPrefab, GeneratedLevelContainer.transform);
                instantiated.transform.position = new Vector3(obj.orginPosx, obj.orginPosy, 0);
                instantiated.GetComponent<Battery>().orginPos = new Vector2(obj.orginPosx, obj.orginPosy);

            }
            else if (obj.type == "Room")
            {
                Debug.Log("Constructing RoomObject");
                GameObject instantiated = Instantiate(roomPrefab, GeneratedLevelContainer.transform);
                instantiated.name = obj.name;
                instantiated.transform.position = new Vector3(obj.positionx, obj.positiony, 0);
                Room room = instantiated.GetComponent<Room>();
                room.defaultRespawn = new Vector2(obj.defaultrespawnx, obj.defaultrespawny);
                room.lockXscroll = obj.lockXscroll;
                room.scrollX = obj.scrollX;
                room.scrollY = obj.scrollY;
                room.minCamX = obj.minCamX;
                room.minCamY = obj.minCamY;
                room.maxCamX = obj.maxCamX;
                room.maxCamY = obj.maxCamY;
                room.lmPlayer = roomLmPlayer;
                room.transitionBoxes = new List<RoomTransitionBBox>();
                foreach (RoomTransitionBBoxSerializable serializablebox in obj.transitionBoxes)
                {
                    RoomTransitionBBox box = new RoomTransitionBBox();
                    box.playerCoords = new Vector2(serializablebox.playerCoordsx, serializablebox.playerCoordsy);
                    box.pos = new Vector2(serializablebox.posx, serializablebox.posy);
                    box.size = new Vector2(serializablebox.sizex, serializablebox.sizey);
                    GameObject transferTo = GameObject.Find(serializablebox.transferTo);
                    if (transferTo)
                    {
                        box.transferTo = transferTo.GetComponent<Room>();
                        room.transitionBoxes.Add(box);
                    }
                    else
                    {
                        Debug.Log("Failed to find transitionbox " + serializablebox.transferTo + ". Retrying in 0.5s");
                        StartCoroutine(RetryBoxFind(room, box, serializablebox.transferTo));
                    }
                }
            }
            else
            {
                Debug.Log("Unable to reconstruct " + obj.type);
            }
        }

        foreach (string tiledata in md.terrainmap)
        {
            string[] splitdata = tiledata.Split("|");
            string tilename = splitdata[0];
            TileBase tile = tiles.Find(def => def.name == tilename).tile;
            if (tile == null){
                Debug.Log(tilename + " was not found in database");
            }
            Vector3Int tilepos = new Vector3Int(System.Int32.Parse(splitdata[2]), System.Int32.Parse(splitdata[3]), 0);
            Matrix4x4 tilematrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, float.Parse(splitdata[1])));
            terrainmap.SetTile(tilepos, tile);
            terrainmap.SetTransformMatrix(tilepos, tilematrix);
        }
        foreach (string tiledata in md.hazardmap)
        {
            string[] splitdata = tiledata.Split("|");
            string tilename = splitdata[0];
            TileBase tile = tiles.Find(def => def.name == tilename).tile;
            if (tile == null){
                Debug.Log(tilename + " was not found in database");
            }
            Vector3Int tilepos = new Vector3Int(System.Int32.Parse(splitdata[2]), System.Int32.Parse(splitdata[3]), 0);
            Matrix4x4 tilematrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, float.Parse(splitdata[1])));
            hazardmap.SetTile(tilepos, tile);
            hazardmap.SetTransformMatrix(tilepos, tilematrix);
        }
        foreach (string tiledata in md.bgmap)
        {
            string[] splitdata = tiledata.Split("|");
            string tilename = splitdata[0];
            TileBase tile = tiles.Find(def => def.name == tilename).tile;
            if (tile == null){
                Debug.Log(tilename + " was not found in database");
            }
            Vector3Int tilepos = new Vector3Int(System.Int32.Parse(splitdata[2]), System.Int32.Parse(splitdata[3]), 0);
            Matrix4x4 tilematrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, float.Parse(splitdata[1])));
            bgmap.SetTile(tilepos, tile);
            bgmap.SetTransformMatrix(tilepos, tilematrix);
        }

    }
    IEnumerator RetryBoxFind(Room room, RoomTransitionBBox box, string transferTo)
    {
        yield return new WaitForSeconds(0.5f);
        GameObject transferToRes = GameObject.Find(transferTo);
        if (transferToRes)
        {
            box.transferTo = transferToRes.GetComponent<Room>();
            room.transitionBoxes.Add(box);
        }
        else
        {
            Debug.Log("Failed to find transitionbox " + transferTo + " again. Retrying in 0.5s");
            StartCoroutine(RetryBoxFind(room, box, transferTo));
        }
    }

    public void SaveLevel()
    {
        MapData md = new MapData();
        md.objs = new List<LevelObject>();
        foreach (Transform trans in mapObjectToSerializeYesThisIsAnotherReallyLongDebugFunctionThatHopefullyWillBeRemovedSoonInALaterPatch.transform)
        {
            GameObject obj = trans.gameObject;
            if (obj.GetComponent<Room>())
            {
                Room room = obj.GetComponent<Room>();
                LevelObject serializableObject = new LevelObject();
                serializableObject.type = "Room";
                serializableObject.positionx = trans.position.x;
                serializableObject.positiony = trans.position.y;
                serializableObject.name = room.name;
                serializableObject.lockXscroll = room.lockXscroll;
                serializableObject.maxCamX = room.maxCamX;
                serializableObject.maxCamY = room.maxCamY;
                serializableObject.minCamX = room.minCamX;
                serializableObject.minCamY = room.minCamY;
                serializableObject.scrollX = room.scrollX;
                serializableObject.scrollY = room.scrollY;
                serializableObject.transitionBoxes = new List<RoomTransitionBBoxSerializable>();
                foreach (RoomTransitionBBox box in room.transitionBoxes)
                {
                    RoomTransitionBBoxSerializable serializableBox = new RoomTransitionBBoxSerializable();
                    serializableBox.playerCoordsx = box.playerCoords.x;
                    serializableBox.playerCoordsy = box.playerCoords.y;
                    serializableBox.sizex = box.size.x;
                    serializableBox.sizey = box.size.y;
                    serializableBox.posx = box.pos.x;
                    serializableBox.posy = box.pos.y;

                    serializableBox.playerCoordsx = box.playerCoords.x;
                    serializableBox.playerCoordsy = box.playerCoords.y;
                    serializableBox.transferTo = box.transferTo.name;
                    serializableObject.transitionBoxes.Add(serializableBox);
                }

                serializableObject.defaultrespawnx = room.defaultRespawn.x;
                serializableObject.defaultrespawny = room.defaultRespawn.y;

                md.objs.Add(serializableObject);
            }
            else if (obj.GetComponent<Battery>())
            {
                LevelObject serializableObject = new LevelObject();
                serializableObject.type = "Battery";
                serializableObject.orginPosx = trans.position.x;
                serializableObject.orginPosy = trans.position.y;

                md.objs.Add(serializableObject);
            }
        }

        string[,] terrain = new string[terrainmap.cellBounds.size.x, terrainmap.cellBounds.size.y];
        Debug.Log(terrainmap.cellBounds.size.x + " : " + terrainmap.cellBounds.size.y);
        for (int x = terrainmap.cellBounds.min.x; x < terrainmap.cellBounds.max.x; x++)
        {
            for (int y = terrainmap.cellBounds.min.y; y < terrainmap.cellBounds.max.y; y++)
            {
                TileBase tile = terrainmap.GetTile(new Vector3Int(x, y, 0));
                if (tile != null)
                {
                    md.terrainmap.Add(tile.name + "|" + terrainmap.GetTransformMatrix(new Vector3Int(x, y, 0)).rotation.eulerAngles.z + "|" + x + "|" + y);
                }
            }
        }

        string[,] hazard = new string[hazardmap.cellBounds.size.x, hazardmap.cellBounds.size.y];
        Debug.Log(hazardmap.cellBounds.size.x + " : " + hazardmap.cellBounds.size.y);
        for (int x = hazardmap.cellBounds.min.x; x < hazardmap.cellBounds.max.x; x++)
        {
            for (int y = hazardmap.cellBounds.min.y; y < hazardmap.cellBounds.max.y; y++)
            {
                TileBase tile = hazardmap.GetTile(new Vector3Int(x, y, 0));
                if (tile != null)
                {
                    md.hazardmap.Add(tile.name + "|" + hazardmap.GetTransformMatrix(new Vector3Int(x, y, 0)).rotation.eulerAngles.z + "|" + x + "|" + y);
                }
            }
        }
        string[,] bg = new string[bgmap.cellBounds.size.x, bgmap.cellBounds.size.y];
        Debug.Log(bgmap.cellBounds.size.x + " : " + bgmap.cellBounds.size.y);
        for (int x = bgmap.cellBounds.min.x; x < bgmap.cellBounds.max.x; x++)
        {
            for (int y = bgmap.cellBounds.min.y; y < bgmap.cellBounds.max.y; y++)
            {
                TileBase tile = bgmap.GetTile(new Vector3Int(x, y, 0));
                if (tile != null)
                {

                    md.bgmap.Add(tile.name + "|" + bgmap.GetTransformMatrix(new Vector3Int(x, y, 0)).rotation.eulerAngles.z + "|" + x + "|" + y);
                }
            }
        }
        //levelToLoadDebugPleaseRemoveLaterSoThatIDontHaveToKeepUsingThisUnnecisarrilyLongVariable = System.Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonUtility.ToJson(md)));
    }
}
