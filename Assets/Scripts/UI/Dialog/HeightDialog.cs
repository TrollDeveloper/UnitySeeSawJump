using System.Collections;
using System.Collections.Generic;
using CodeControl;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Dialog
{
    public class HeightDialog : DialogBase
    {
        private GameContentModel contentModel;
        [SerializeField] private Text text;

        // Start is called before the first frame update
        void Start()
        {
            contentModel = Model.First<GameContentModel>();
        }


        // Update is called once per frame
        void Update()
        {
            text.text = $"CurHeight : {contentModel.characterHeight}\n" +
                        $"Energy : {((float) contentModel.getItem / contentModel.totalItem)}";
        }
    }
}