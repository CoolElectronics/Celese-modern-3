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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (convertTilesToBase64){
            convertTilesToBase64 = false;
            MapData md = new MapData();
            string[,] terrain = new string[terrainmap.cellBounds.size.x,terrainmap.cellBounds.size.y];
            Debug.Log(terrainmap.cellBounds.size.x + " : "+ terrainmap.cellBounds.size.y);
            for (int x = terrainmap.cellBounds.min.x; x < terrainmap.cellBounds.max.x; x++) {
            for (int y = terrainmap.cellBounds.min.y; y < terrainmap.cellBounds.max.y; y++) {
                TileBase tile = terrainmap.GetTile(new Vector3Int(x,y,0));
                if (tile != null) {
                    //terrainmap.SetTransformMatrix(new Vector3Int(x,y,0),Matrix4x4.Rotate(Quaternion.Euler(0,0,90)));
                    md.terrainmap.Add(tile.name + "|" + terrainmap.GetTransformMatrix(new Vector3Int(x,y,0)).rotation.eulerAngles.z + "|" + x + "|" + y);
                }
            }
        }  

        // jesus christ this is a mess
        levelToLoadDebugPleaseRemoveLaterSoThatIDontHaveToKeepUsingThisUnnecisarrilyLongVariable = Newtonsoft.Json.JsonConvert.SerializeObject(md);
        //Newtonsoft.Json.JsonConvert.SerializeObject(CLZF2.Compress(System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(md))));
        Debug.Log(levelToLoadDebugPleaseRemoveLaterSoThatIDontHaveToKeepUsingThisUnnecisarrilyLongVariable);
        }
        if (convertTilesFromBase64){
            convertTilesFromBase64 = false;

        }
    }
    public static void CopyTo(Stream src, Stream dest) {
    byte[] bytes = new byte[4096];

    int cnt;

    while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0) {
        dest.Write(bytes, 0, cnt);
    }
}

public static byte[] Zip(string str) {
    var bytes = Encoding.UTF8.GetBytes(str);

    using (var msi = new MemoryStream(bytes))
    using (var mso = new MemoryStream()) {
        using (var gs = new GZipStream(mso, CompressionMode.Compress)) {
            //msi.CopyTo(gs);
            CopyTo(msi, gs);
        }

        return mso.ToArray();
    }
}

public static string Unzip(byte[] bytes) {
    using (var msi = new MemoryStream(bytes))
    using (var mso = new MemoryStream()) {
        using (var gs = new GZipStream(msi, CompressionMode.Decompress)) {
            //gs.CopyTo(mso);
            CopyTo(gs, mso);
        }

        return Encoding.UTF8.GetString(mso.ToArray());
    }
}

}
