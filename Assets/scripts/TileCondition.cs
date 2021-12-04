using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[System.Serializable]
public struct TileCondition
{
    public bool requireSetTile;
    public List<TileBase> requirementTile;
    public Vector3Int offset;
    public ConditionType type;
}