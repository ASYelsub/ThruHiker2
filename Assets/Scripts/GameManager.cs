using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Things to add:
//-multiple paths, not just one.
//-have side to side vary in height
//-maybe player can sit on rocks
//-variation of tree density, let trees be more dense closer to path
//-have hikers spawn in from top
//-"herding" childeren, as game design focus (going off paths, different speeds)

public class GameManager : MonoBehaviour
{
    [HideInInspector] public Level currentLevel;
    public List<Level> levels = new List<Level>();
    public void Awake()
    {
        Services.Game = this;
    }
    public void Start()
    {
        Services.InitServices();
    }

    public void Init()
    {
        SetLevel();
    }
    public void SetLevel()
    {
        currentLevel = levels[0];
        Services.HikerGenerator.ResetHikerSelection();
    }

}
