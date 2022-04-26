using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Notes from Shawn's class
public class BaseBehaviour : MonoBehaviour
{
    ///Returns true if a number is between a certain range (Inclusive)///
    protected bool IsBetween(int n, int x, int y)
    {
        if (n < y)
            return x > n && x < y;
        else
            return x > y && x < n;
    }
}
