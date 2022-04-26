using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Hiker", menuName = "Scriptables/Hiker", order = 1)]
public class Hiker : ScriptableObject
{
    public string firstName, lastName;
    public List<Level> appearanceLevels = new List<Level>();

    public void AddLevel()
    {
        // appearanceLevels.Add(new Level("New level " + appearanceLevels.Count, appearanceLevels.Count));
    }
}
