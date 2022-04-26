using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Holds data for each "spot" on the trail that a path is generated to/from.

[System.Serializable]
[CreateAssetMenu(fileName = "TrailSpot", menuName = "Scriptables/TrailSpot", order = 1)]
public class TrailSpot : ScriptableObject
{
    public string locationName;
    public Vector2 mapPosition;
    public float altitutde;

}

public enum TrailSpotType { Town, Lookout, Shelter, River, Lake }
