using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class GlobalModelHolder : MonoBehaviour
{
    ModelRef<GameContentModel> contentModel;
    ModelRef<GameSettingModel> settingModel;

    private void Awake()
    {
        contentModel = new ModelRef<GameContentModel>(new GameContentModel());
        contentModel.Model.Init();

        settingModel = new ModelRef<GameSettingModel>(new GameSettingModel());
        settingModel.Model.Init();
    }
}