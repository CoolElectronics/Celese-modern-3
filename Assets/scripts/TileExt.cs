using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class TileExt
{
    public Vector3Int pos;
    public TileBase tile;
    public Tilemap tilemap;
    public AutotilerPreset preset;
    public TileExt(Vector3Int basePos, Tilemap baseTilemap, AutotilerPreset presets)
    {
        tilemap = baseTilemap;
        pos = basePos;
        tile = tilemap.GetTile(pos);
        preset = presets;
    }
    public void Update()
    {
        TileBase selectedTile = preset.icon;
        int rot = 0;
        for (int a = preset.rules.Count - 1; a > -1; a--)
        {
            for (int r = (preset.rules[a].rotateConditions ? 3 : 0); r > -1; r--)
            {
                int trueconditions = 0;
                int maxtrueconditions = 0;
                for (int b = preset.rules[a].conditions.Count - 1; b > -1; b--)
                {
                    if (preset.rules[a].conditions[b].type == ConditionType.REQUIRED)
                    {
                        maxtrueconditions++;
                    }
                    if (Condition(preset.rules[a].conditions[b], r))
                    {
                        if (preset.rules[a].conditions[b].type == ConditionType.REQUIRED)
                        {
                            trueconditions++;
                        }
                        else if (preset.rules[a].conditions[b].type == ConditionType.INVALID)
                        {
                            trueconditions = -100;
                        }
                    }
                    else
                    {
                        if (preset.rules[a].conditions[b].type == ConditionType.REQUIRED)
                        {
                            trueconditions--;
                        }
                    }
                }
                if (trueconditions >= maxtrueconditions)
                {
                    selectedTile = preset.rules[a].tiles[Random.Range(0, preset.rules[a].tiles.Count)];
                    rot = preset.rules[a].rotation + r;
                }
            }
        }
        tilemap.SetTile(pos, selectedTile);
        Matrix4x4 tilematrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, rot * 90));
        tilemap.SetTransformMatrix(pos, tilematrix);
    }

    public bool Condition(TileCondition con, int r)
    {

        //scuffed
        TileBase selectedTile = tilemap.GetTile(pos + rotateInt(con.offset, r));
        //pos + tilemap.WorldToCell(Quaternion.Euler(0, 0, r * 90) * tilemap.CellToWorld(con.offset))
        //Debug.Log("checking tile alignment for rotation depth " + pos + con.offset + " : " + pos + tilemap.WorldToCell(Quaternion.Euler(0, 0, r * 90) * tilemap.CellToWorld(con.offset)));
        //Debug.Log(selectedTile + " : " + r);
        if (selectedTile != null)
        {
            if (con.requirementTile.Contains(selectedTile) || !con.requireSetTile)
            {

                return true;
            }
        }
        return false;
    }
    public Vector3Int rotateInt(Vector3Int vec, int depth)
    {
        Vector3Int newVec = vec;
        if (depth > 0)
        {
            newVec.x = -vec.y;
            newVec.y = vec.x;
            return rotateInt(newVec, depth - 1);

        }
        else
        {
            return vec;
        }

    }
    public void Remove()
    {
        tilemap.SetTile(pos, null);
    }
}