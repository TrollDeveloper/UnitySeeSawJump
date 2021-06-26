using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeControl;
using Content;

namespace UI.Dialog
{
    public class JumpingDialog : DialogBase
    {
        public Slider jumpingSlider;
        public RectTransform sliderParent;
        public RectTransform normalGage;
        public RectTransform perfectGage;

        private void OnEnable()
        {
            Message.AddListener<UpdateJumpingGageMsg>(OnUpdateJumpingGageMsg);
            Message.AddListener<SetJumpingGageInfoMsg>(OnSetJumpingGageInfoMsg);
            jumpingSlider.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            Message.RemoveListener<UpdateJumpingGageMsg>(OnUpdateJumpingGageMsg);
            Message.RemoveListener<SetJumpingGageInfoMsg>(OnSetJumpingGageInfoMsg);
        }

        void OnUpdateJumpingGageMsg(UpdateJumpingGageMsg msg)
        {
            jumpingSlider.value = msg.gage;
        }

        void OnSetJumpingGageInfoMsg(SetJumpingGageInfoMsg msg)
        {
            float width = sliderParent.rect.width;

            normalGage.offsetMin = new Vector2(msg.minNormal * width, 0f);
            normalGage.offsetMax = new Vector2(-(1f - msg.maxNormal) * width, 0f);
            perfectGage.offsetMin = new Vector2(msg.minPerfect * width, 0f);
            perfectGage.offsetMax = new Vector2(-(1f - msg.maxPerfect) * width, 0f);
            jumpingSlider.gameObject.SetActive(true);
        }
    }
}