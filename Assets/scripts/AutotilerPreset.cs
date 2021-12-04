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
    public Sprite iconSprite;
    public TileBase icon;
    public List<PresetRule> rules;
}