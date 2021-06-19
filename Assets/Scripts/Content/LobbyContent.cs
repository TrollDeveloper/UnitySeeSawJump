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
        Message.Send(new RequestGameStateDialogEnterMsg(GameStateManager.State.Lobby));
        Message.Send(new CleanUpAllItemMsg());

        Message.AddListener<StartButtonClickMsg>(OnStartButtonClickMsg);
    }

    public override void Exit()
    {
        base.Exit();
        //Turn Off Lobby UI.
        Message.Send(new RequestGameStateDialogExitMsg(GameStateManager.State.Lobby));

        MessageHelper.RemoveListenerEndFrame<StartButtonClickMsg>(OnStartButtonClickMsg);
    }

    void OnStartButtonClickMsg(StartButtonClickMsg msg)
    {
        //ChangeState To Intro.
        Message.Send(new GameStateChangeMsg(GameStateManager.State.Intro));
    }
}
