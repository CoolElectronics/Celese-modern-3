using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[System.Serializable]
public struct AutotilerPreset
{
    public Button button;
    public string name;
    public Sprite icon;
    public int layer;

    public List<TileBase> flat;
    public List<TileBase> mid1;
    public List<TileBase> mid2;
    public List<TileBase> mid3;
    public List<TileBase> corner;
    public List<TileBase> innercorner;


}