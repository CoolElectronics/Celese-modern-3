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
    void Awake()
    {
        i = this;
    }

    public void Paint(Vector3 pos, AutotilerPreset preset)
    {

        terrainMap.SetTile(terrainMap.WorldToCell(pos), preset.flat[0]);
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
        int depthx = 0;
        int depthy = 0;


        for (float x = minx; x < maxx; x += minx + 0.5f < maxx && depthx == 0 ? 0 : 0.5f)
        {
            Debug.Log(x);
            depthx++;
            depthy = 0;
            for (float y = miny; y < maxy; y += miny + 0.5f < maxy && depthy == 0 ? 0 : 0.5f)
            {
                depthy++;
                Debug.Log(y);
                terrainMap.SetTile(terrainMap.WorldToCell(new Vector3(x, y, 0)), preset.flat[0]);
            }
        }
    }
}
