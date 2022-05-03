using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Services
{
    private static GameManager _gameManager;
    public static GameManager Game
    {
        get
        {
            Debug.Assert(_gameManager != null);
            return _gameManager;
        }
        set => _gameManager = value;
    }

    private static TrailGenerator _trailGenerator;
    public static TrailGenerator TrailGenerator
    {
        get
        {
            Debug.Assert(_trailGenerator != null);
            return _trailGenerator;
        }
        set => _trailGenerator = value;
    }

    private static HikerGenerator _hikerGenerator;
    public static HikerGenerator HikerGenerator
    {
        get
        {
            Debug.Assert(_hikerGenerator != null);
            return _hikerGenerator;
        }
        set => _hikerGenerator = value;
    }

    private static Player _player;
    public static Player Player
    {
        get
        {
            Debug.Assert(_player != null);
            return _player;
        }
        set => _player = value;
    }



    public static void InitServices()
    {
        Game.Init();
        TrailGenerator.Init();
        Player.Init();
        HikerGenerator.Init();
    }

}
