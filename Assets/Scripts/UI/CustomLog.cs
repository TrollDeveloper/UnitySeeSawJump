using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomLog : MonoBehaviourSingleton<CustomLog>
{
    public Text text;

    private List<string> dataList = new List<string>();

    public void AddLog(string str)
    {
        dataList.Add(str);
        if (dataList.Count > 10)
        {
            dataList.RemoveAt(0);
        }

        text.text = "";
        for (int i = 0; i < dataList.Count; i++)
        {
            text.text += dataList[i] + '\n';
        }
    }

    private void Update()
    {
        if (Input.touchCount == 4)
        {
            text.gameObject.SetActive(!text.gameObject.activeSelf);
        }
    }
}