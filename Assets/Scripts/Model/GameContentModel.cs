using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
public class GameContentModel : Model
{
    public float targetHeight;
    public float characterHeight;
    public float energy;

    public void Init()
    {
        targetHeight = 50f;
    }

    public void Reset()
    {

    }
}
