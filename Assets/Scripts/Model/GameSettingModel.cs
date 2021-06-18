using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
public class GameSettingModel : Model
{
    public enum ControlType
    {
        TargetPoint,
        Direction,
    }

    public ControlType controlType;
    
    public void Init()
    {
        controlType = ControlType.Direction;
    }
}
