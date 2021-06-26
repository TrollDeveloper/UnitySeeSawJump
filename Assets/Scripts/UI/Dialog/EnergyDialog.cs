using System;
using System.Collections;
using System.Collections.Generic;
using CodeControl;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using CodeControl;
using TMPro;

namespace UI.Dialog
{
    public class EnergyDialog : DialogBase
    {
        private GameContentModel contentModel;
        [SerializeField] private TextMeshProUGUI energyText;
        [SerializeField] private Slider energySlider;
        [SerializeField] private RectTransform successThresholdUI;

        // Start is called before the first frame update
        void Start()
        {
            contentModel = Model.First<GameContentModel>();
            successThresholdUI.anchoredPosition =
                new Vector2(0, -
                                   energySlider.image.rectTransform.sizeDelta.y *
                               (1f - Model.First<GameSettingModel>().energySuccessThreshold));
        }

        private void Update()
        {
            float energyRatio = (float) contentModel.curEnergy / contentModel.totalEnergy;
            energyText.text = $"{((energyRatio) * 100f):n0} \n/\n100";
            energySlider.value = energyRatio;
        }
    }
}