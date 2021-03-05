using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileData : ScriptableObject
{
    // should I make these private and create get/set functions?
    public TileBase[] tiles;
    public string terrain;
}
