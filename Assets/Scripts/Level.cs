using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = "Level", menuName = "Scriptables/Level", order = 1)]
public class Level : ScriptableObject
{
    public int levelNumber;
    public string levelName;
    public TrailSpot startTrailSpot, endTrailSpot;
    public float levelDistance;
    public AnimationCurve altitudeCurve;
    public float trailDistance;
    public float trailWidth;
    // public float levelAltitude;

}
