using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TextObject : LevelObject{
   public Vector3 specialpos;
   public Vector2 scale;
   public Vector3 realscale;

   public float fontSize;
}