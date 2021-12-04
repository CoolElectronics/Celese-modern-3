using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[System.Serializable]
public struct PresetRule
{
    public List<TileBase> tiles;
    public List<TileCondition> conditions;
    public RotationType rotation;

    public bool rotateConditions;
}