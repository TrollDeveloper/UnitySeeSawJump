using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using CodeControl;
using EasyMobile.Demo;

public class RequestGameStateDialogEnterMsg : Message
{
    public GameStateManager.State state;
    public RequestGameStateDialogEnterMsg(GameStateManager.State state)
    {
        this.state = state;
    }
}
public class RequestGameStateDialogExitMsg : Message
{
    public GameStateManager.State state;
    public RequestGameStateDialogExitMsg(GameStateManager.State state)
    {
        this.state = state;
    }
}

public class UIManager : SerializedMonoBehaviour
{
    [OdinSerialize]
    Dictionary<GameStateManager.State, List<GameObject>> uiPanelMap =
        new Dictionary<GameStateManager.State, List<GameObject>>();

    private void Awake()
    {
        foreach (var keyValue in uiPanelMap)
        {
            var list = keyValue.Value;
            for (int i = 0; i < list.Count; i++)
            {
                list[i].SetActive(false);
            }

        }

        Message.AddListener<RequestGameStateDialogEnterMsg>(OnRequestGameStateDialogEnterMsg);
        Message.AddListener<RequestGameStateDialogExitMsg>(OnRequestGameStateDialogExitMsg);
    }

    private void OnDestroy()
    {
        Message.RemoveListener<RequestGameStateDialogEnterMsg>(OnRequestGameStateDialogEnterMsg);
        Message.RemoveListener<RequestGameStateDialogExitMsg>(OnRequestGameStateDialogExitMsg);
    }

    void OnRequestGameStateDialogEnterMsg(RequestGameStateDialogEnterMsg msg)
    {
        SetActiveUI(msg.state, true);
    }

    void OnRequestGameStateDialogExitMsg(RequestGameStateDialogExitMsg msg)
    {
        SetActiveUI(msg.state, false);
    }

    void SetActiveUI(GameStateManager.State state, bool isActive)
    {
        if (uiPanelMap.ContainsKey(state))
        {
            var list = uiPanelMap[state];
            for (int i = 0; i < list.Count; i++)
            {
                list[i].SetActive(isActive);
            }
        }
    }
}
