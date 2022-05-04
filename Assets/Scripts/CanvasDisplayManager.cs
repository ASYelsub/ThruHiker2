using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasDisplayManager : MonoBehaviour
{
    public Map map;

    void Awake()
    {
        Services.CanvasDisplay = this;
    }
    public void Init()
    {

    }
}
