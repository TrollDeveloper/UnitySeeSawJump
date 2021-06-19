using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class PauseButtonClickMsg : VoidMessageBase
{
    public override void Send()
    {
        Message.Send(this);
    }
}

public class PauseResumeButtonClickMsg : VoidMessageBase
{
    public override void Send()
    {
        Message.Send(this);
    }
}

public class PauseRestartButtonClickMsg : VoidMessageBase
{
    public override void Send()
    {
        Message.Send(this);
    }
}

public class PauseLobbyButtonClickMsg : VoidMessageBase
{
    public override void Send()
    {
        Message.Send(this);
    }
}

public class PauseDialog : MonoBehaviour
{
    public GameObject pausePopup;

    private void OnEnable()
    {
        pausePopup.SetActive(false);
        Message.AddListener<PauseButtonClickMsg>(OnPauseButtonClickMsg);
        Message.AddListener<PauseResumeButtonClickMsg>(OnPauseResumeButtonClickMsg);
        Message.AddListener<PauseRestartButtonClickMsg>(OnPauseRestartButtonClickMsg);
        Message.AddListener<PauseLobbyButtonClickMsg>(OnPauseLobbyButtonClickMsg);
    }

    private void OnDisable()
    {
        MessageHelper.RemoveListenerEndFrame<PauseButtonClickMsg>(OnPauseButtonClickMsg);
        MessageHelper.RemoveListenerEndFrame<PauseResumeButtonClickMsg>(OnPauseResumeButtonClickMsg);
        MessageHelper.RemoveListenerEndFrame<PauseRestartButtonClickMsg>(OnPauseRestartButtonClickMsg);
        MessageHelper.RemoveListenerEndFrame<PauseLobbyButtonClickMsg>(OnPauseLobbyButtonClickMsg);
        Time.timeScale = 1f;
    }

    void OnPauseButtonClickMsg(PauseButtonClickMsg msg)
    {
        pausePopup.SetActive(true);
        Time.timeScale = 0f;
    }

    void OnPauseResumeButtonClickMsg(PauseResumeButtonClickMsg msg)
    {
        pausePopup.SetActive(false);
        Time.timeScale = 1f;
    }

    void OnPauseRestartButtonClickMsg(PauseRestartButtonClickMsg msg)
    {
        Message.Send(new GameStateChangeMsg(GameStateManager.State.Intro));
    }

    void OnPauseLobbyButtonClickMsg(PauseLobbyButtonClickMsg msg)
    {
        Message.Send(new GameStateChangeMsg(GameStateManager.State.Lobby));
    }
}