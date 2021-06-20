using System;
using System.Collections;
using System.Collections.Generic;
using Beebyte.Obfuscator;
using UnityEngine;
using CodeControl;
using Ludiq.OdinSerializer;

namespace UI.Dialog
{
    [SkipRename]
    public class StartButtonClickMsg : VoidMessageBase
    {
        public override void Send()
        {
            Message.Send(this);
        }
    }
    [SkipRename]
    public class SettingPopupCloseButtonClickMsg : VoidMessageBase
    {
        public override void Send()
        {
            Message.Send(this);
        }
    }

    [SkipRename]
    public class SettingChangeControlTypeButtonClickMsg : VoidMessageBase
    {
        [OdinSerialize] public GameSettingModel.ControlType type;

        public override void Send()
        {
            Message.Send(this);
        }
    }


    public class LobbyDialog : DialogBase
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
}