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
        RotationType rot = RotationType.NONE;
        for (int a = 0; a < preset.rules.Count; a++)
        {
            for (int r = 0; r < (preset.rules[a].rotateConditions ? 4 : 1); r++)
            {
                int trueconditions = 0;
                int maxtrueconditions = 0;
                for (int b = 0; b < preset.rules[a].conditions.Count; b++)
                {
                    if (preset.rules[a].conditions[b].type == ConditionType.REQUIRED)
                    {
                        maxtrueconditions++;
                    }
                    if (Condition(preset.rules[a].conditions[b],r))
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
                    rot = preset.rules[a].rotation;
                }
            }
        }
        tilemap.SetTile(pos, selectedTile);
        if (rot != RotationType.NONE)
        {
            Matrix4x4 tilematrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 0));
            switch (rot)
            {
                case RotationType.RIGHT:
                    tilematrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 90));
                    break;
                case RotationType.DOWN:
                    tilematrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 180));
                    break;
                case RotationType.LEFT:
                    tilematrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 270));
                    break;
            }
            tilemap.SetTransformMatrix(pos, tilematrix);
        }
    }

    public bool Condition(TileCondition con,int r)
    {
        Debug.Log("checking tile alignment for rotation depth " + r);
        //scuffed
        TileBase selectedTile = tilemap.GetTile(pos + tilemap.WorldToCell(Quaternion.Euler(0, 0, r * 90) * tilemap.CellToWorld(con.offset)));
        if (selectedTile != null)
        {
            if (con.requirementTile.Contains(selectedTile) || !con.requireSetTile)
            {
                return true;
            }
        }
        return false;
    }
    public void Remove()
    {
        tilemap.SetTile(pos, null);
    }
}