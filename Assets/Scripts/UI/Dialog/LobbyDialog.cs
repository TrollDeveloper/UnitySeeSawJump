using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class SettingPopupCloseButtonClickMsg : VoidMessageBase
{
    public override void Send()
    {
        Message.Send(this);
    }
}

public class SettingChangeControlTypeButtonClickMsg : VoidMessageBase
{
    public GameSettingModel.ControlType type;

    public override void Send()
    {
        Message.Send(this);
    }
}


public class LobbyDialog : MonoBehaviour
{
    public GameObject settingPopup;

    private void OnEnable()
    {
        settingPopup.SetActive(false);
        Message.AddListener<SettingPopupCloseButtonClickMsg>(OnSettingPopupCloseButtonClickMsg);
        Message.AddListener<SettingChangeControlTypeButtonClickMsg>(OnSettingChangeControlTypeButtonClickMsg);
    }

    private void OnDisable()
    {
        Message.RemoveListener<SettingPopupCloseButtonClickMsg>(OnSettingPopupCloseButtonClickMsg);
        Message.RemoveListener<SettingChangeControlTypeButtonClickMsg>(OnSettingChangeControlTypeButtonClickMsg);
    }

    void OnSettingPopupCloseButtonClickMsg(SettingPopupCloseButtonClickMsg clickMsg)
    {
        //TODO : Save Setting.
        settingPopup.SetActive(false);
    }

    void OnSettingChangeControlTypeButtonClickMsg(SettingChangeControlTypeButtonClickMsg msg)
    {
        Model.First<GameSettingModel>().controlType = msg.type;
    }
}