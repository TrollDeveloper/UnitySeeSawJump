using System;
using System.Collections;
using System.Collections.Generic;
using Beebyte.Obfuscator;
using UnityEngine;
using CodeControl;

[SkipRename]
public class GameSettingModel : Model
{
    [Skip]
    public enum ControlType
    {
        TargetPoint,
        Direction,
    }

    public ControlType controlType;

    public void Init()
    {
        controlType = ControlType.TargetPoint;
    }
}