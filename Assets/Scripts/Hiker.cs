using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hiker : MonoBehaviour
{
    public string firstName;
    public string lastName;
    public List<Level> appearanceLevels = new List<Level>();
    public void Init(string firstName, string lastName)
    {
        this.firstName = firstName;
        this.lastName = lastName;
    }
    public void AddLevel()
    {
        // appearanceLevels.Add(new Level("New level " + appearanceLevels.Count, appearanceLevels.Count));
    }
}
