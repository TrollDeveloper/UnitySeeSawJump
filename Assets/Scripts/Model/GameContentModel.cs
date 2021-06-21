using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class GameContentModel : Model
{
    public float targetHeight;
    public float characterHeight;
    public int curEnergy;
    public int totalEnergy;

    public void Init()
    {
        targetHeight = 50f;
        Reset();
    }

    public void Reset()
    {
        curEnergy = 0;
        totalEnergy = 0;
    }
}