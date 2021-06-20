using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class LobbyContent : InGameContentBase
{
    public override void Enter()
    {
        base.Enter();
        //Set Character Lobby Position.
        Message.Send(new CharacterChangeStateFromGameStateMsg(GameStateManager.State.Lobby));
        //Set Camera SeeSaw Position.
        Message.Send(new CameraStateChangeMsg(CameraController.State.SeeSaw));
        //Turn On Lobby UI. 
        UIManager.Instance.RequestDialogEnter<LobbyDialog>();
        UIManager.Instance.RequestDialogExit<PauseDialog>();
        UIManager.Instance.RequestDialogExit<HeightDialog>();

        Message.Send(new CleanUpAllItemMsg());

        Message.AddListener<TouchDownMsg>(OnTouchDownMsg);
        Message.AddListener<StartButtonClickMsg>(OnStartButtonClickMsg);
    }

    public override void Exit()
    {
        base.Exit();
        //Turn Off Lobby UI.
        UIManager.Instance.RequestDialogExit<LobbyDialog>();

        MessageHelper.RemoveListenerEndFrame<TouchDownMsg>(OnTouchDownMsg);
        MessageHelper.RemoveListenerEndFrame<StartButtonClickMsg>(OnStartButtonClickMsg);
    }

    void GameStart()
    {
        //ChangeState To Intro.
        Message.Send(new GameStateChangeMsg(GameStateManager.State.Intro));
    }

    void OnTouchDownMsg(TouchDownMsg msg)
    {
        GameStart();
    }

    void OnStartButtonClickMsg(StartButtonClickMsg msg)
    {
        GameStart();
    }
}