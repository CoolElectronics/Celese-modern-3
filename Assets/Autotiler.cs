using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class Autotiler : MonoBehaviour
{
    public List<AutotilerPreset> autotilers = new List<AutotilerPreset>();
    public static Autotiler i;

    [SerializeField]
    Tilemap terrainMap;
    [SerializeField]
    Tilemap hazardMap;
    [SerializeField]
    Tilemap bgMap;
    public Dictionary<Vector3Int, TileExt> terrainTileExts = new Dictionary<Vector3Int, TileExt>();
    public Dictionary<Vector3Int, TileExt> hazardTileExts = new Dictionary<Vector3Int, TileExt>();
    public Dictionary<Vector3Int, TileExt> bgTileExts = new Dictionary<Vector3Int, TileExt>();

    void Awake()
    {
        i = this;
    }

    public void Paint(Vector3 pos, AutotilerPreset preset)
    {
        terrainTileExts.Add(terrainMap.WorldToCell(pos), new TileExt(terrainMap.WorldToCell(pos), terrainMap, preset));
        UpdateTileExts();
    }
    public void UpdateTileExts()
    {
        foreach (KeyValuePair<Vector3Int, TileExt> te in terrainTileExts)
        {
            te.Value.Update();
        }
        foreach (KeyValuePair<Vector3Int, TileExt> te in hazardTileExts)
        {
            te.Value.Update();
        }
        foreach (KeyValuePair<Vector3Int, TileExt> te in bgTileExts)
        {
            te.Value.Update();
        }
    }
    public void DeleteTileExt(Vector3Int pos, Tilemap map)
    {
        TileExt te = terrainTileExts[pos];
        if (te != null)
        {
            if (te.tilemap == map)
            {
                te.Remove();
                terrainTileExts.Remove(pos);
            }
        }
        te = null;

        te = hazardTileExts[pos];
        if (te != null)
        {
            if (te.tilemap == map)
            {
                te.Remove();
                hazardTileExts.Remove(pos);
            }
        }
        te = null;

        te = bgTileExts[pos];
        if (te != null)
        {
            if (te.tilemap == map)
            {
                te.Remove();
                bgTileExts.Remove(pos);
            }
        }
    }
    public void PaintBlock(Vector3 start, Vector3 end, AutotilerPreset preset)
    {
        Vector3Int startint = terrainMap.WorldToCell(start);
        Vector3Int endint = terrainMap.WorldToCell(end);
        Debug.Log(startint + " To " + endint);
        float minx = Mathf.Min(start.x, end.x);
        float miny = Mathf.Min(start.y, end.y);
        float maxx = Mathf.Max(start.x, end.x);
        float maxy = Mathf.Max(start.y, end.y);


        // this took over 2 hours
        // seems so simple


        float x = minx;
        float y = miny;
        while (true)
        {

            if (x > maxx) break;
            x += 0.5f;
            Debug.Log(x);
            y = miny;
            while (true)
            {
                if (y > maxy) break;
                y += 0.5f;
                Debug.Log(y);
                Vector3Int pos = terrainMap.WorldToCell(new Vector3(x - 0.5f, y - 0.5f, 0));
                if (preset.icon)
                {
                    //fix this pls
                    Debug.LogError("not implemented");
                    terrainTileExts.Add(pos,new TileExt(pos, terrainMap, preset));
                }
                else
                {
                    DeleteTileExt(pos, terrainMap);
                }

            }

        }
        UpdateTileExts();
    }
}
