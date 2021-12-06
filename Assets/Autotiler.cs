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
        InvokeRepeating("UpdateTileExts", 1f, 2f);
    }

    public void Paint(Vector3 pos, AutotilerPreset preset)
    {
        Dictionary<Vector3Int, TileExt> exts = TilemapToTileExtDict(preset.layer);
        if (preset.icon)
        {
            if (!exts.ContainsKey(preset.layer.WorldToCell(pos)))
            {
                exts.Add(preset.layer.WorldToCell(pos), new TileExt(preset.layer.WorldToCell(pos), preset.layer, preset));
            }
        }
        else
        {
            DeleteTileExt(terrainMap.WorldToCell(pos));
        }

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
    public void DeleteTileExt(Vector3Int pos)
    {

        if (terrainTileExts.ContainsKey(pos))
        {
            terrainTileExts[pos].Remove();
            terrainTileExts.Remove(pos);
        }
        if (hazardTileExts.ContainsKey(pos))
        {
            hazardTileExts[pos].Remove();
            hazardTileExts.Remove(pos);
        }
        if (bgTileExts.ContainsKey(pos))
        {
            bgTileExts[pos].Remove();
            bgTileExts.Remove(pos);
        }
    }
    public void PaintBlock(Vector3 start, Vector3 end, AutotilerPreset preset)
    {
        Dictionary<Vector3Int, TileExt> exts = TilemapToTileExtDict(preset.layer);
        Vector3Int startint = terrainMap.WorldToCell(start);
        Vector3Int endint = terrainMap.WorldToCell(end);
        //Debug.Log(startint + " To " + endint);
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
            //Debug.Log(x);
            y = miny;
            while (true)
            {
                if (y > maxy) break;
                y += 0.5f;
                //Debug.Log(y);
                Vector3Int pos = terrainMap.WorldToCell(new Vector3(x - 0.5f, y - 0.5f, 0));
                if (preset.icon)
                {
                    //fix this pls
                    if (!exts.ContainsKey(pos))
                    {
                        exts.Add(pos, new TileExt(pos, preset.layer, preset));
                    }
                }
                else
                {
                    DeleteTileExt(pos);
                }

            }

        }
        UpdateTileExts();
    }
    public Dictionary<Vector3Int, TileExt> TilemapToTileExtDict(Tilemap tm)
    {
        if (tm == terrainMap)
        {
            return terrainTileExts;
        }
        else if (tm == hazardMap)
        {
            return hazardTileExts;
        }
        else if (tm == bgMap)
        {
            return bgTileExts;
        }
        else
        {
            return null;
        }
    }
}
