using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace UI.Dialog
{
    public class DownfallDialog : DialogBase
    {
        private GameSettingModel.ControlType controlType;
        public GameObject[] controlPanelArray;

        private float lastTouchPointX = -999f;

        public RectTransform sliderHandle;
        public RectTransform targetPointLine;
        public Image leftDirectionButton;
        public Image rightDirectionbutton;

        public void OnEnable()
        {
            var settingModel = Model.First<GameSettingModel>();
            if (settingModel != null)
            {
                controlType = settingModel.controlType;
            }

            controlPanelArray[(int) controlType].SetActive(true);

            Message.AddListener<MoveLastFingerPositionMsg>(OnMoveLastFingerPositionMsg);
        }

        public void OnDisable()
        {
            controlPanelArray[(int) controlType].SetActive(false);

            Message.RemoveListener<MoveLastFingerPositionMsg>(OnMoveLastFingerPositionMsg);
        }

        public void Update()
        {
            leftDirectionButton.color = Color.red;
            rightDirectionbutton.color = Color.red;

            sliderHandle.anchoredPosition = new Vector2(0f, sliderHandle.anchoredPosition.y);
            targetPointLine.gameObject.SetActive(false);

            if (lastTouchPointX == -999f)
            {
                return;
            }

            switch (controlType)
            {
                case GameSettingModel.ControlType.Direction:
                    (lastTouchPointX < 0.5f ? leftDirectionButton : rightDirectionbutton).color = Color.red * 0.8f;
                    break;
                case GameSettingModel.ControlType.TargetPoint:
                    targetPointLine.gameObject.SetActive(true);
                    Vector3 linePos = targetPointLine.transform.position;
                    linePos.x = lastTouchPointX * Screen.width;
                    targetPointLine.transform.position = linePos;
                    break;
                case GameSettingModel.ControlType.Slider:
                    Vector3 sliderPos = sliderHandle.transform.position;
                    sliderPos.x = lastTouchPointX * Screen.width;
                    sliderHandle.transform.position = sliderPos;
                    break;
            }

            lastTouchPointX = -999f;
        }

        void OnMoveLastFingerPositionMsg(MoveLastFingerPositionMsg msg)
        {
            lastTouchPointX = msg.viewportPosition.x;
        }
    }
}