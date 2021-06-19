using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class GameEndContent : InGameContentBase
{
    public override void Enter()
    {
        base.Enter();
        //GameEnd UI ON.
        Message.Send(new RequestGameStateDialogEnterMsg(GameStateManager.State.GameEnd));
        //Retry Timer ON.

        Message.AddListener<RetryButtonClickMsg>(OnRetryButtonClickMsg);
        Message.AddListener<RestartButtonClickMsg>(OnRestartButtonClickMsg);
        Message.AddListener<GoBackLobbyButtonClickMsg>(OnGoBackLobbyButtonClickMsg);
    }
    public override void Exit()
    {
        base.Exit();
        StopAllCoroutines();
        //GameEnd UI Off.
        Message.Send(new RequestGameStateDialogExitMsg(GameStateManager.State.GameEnd));

        MessageHelper.RemoveListenerEndFrame<RetryButtonClickMsg>(OnRetryButtonClickMsg);
        MessageHelper.RemoveListenerEndFrame<RestartButtonClickMsg>(OnRestartButtonClickMsg);
        MessageHelper.RemoveListenerEndFrame<GoBackLobbyButtonClickMsg>(OnGoBackLobbyButtonClickMsg);
    }
    IEnumerator RetryCoroutine()
    {
        //Score Rollback.
        //Fade out-> ChangeState to Rocket.

        Message.Send(new CameraFadeOutMsg(0.5f));
        yield return new WaitForSeconds(1f);
        Message.Send(new GameStateChangeMsg(GameStateManager.State.Rocket));
    }
    void OnRetryButtonClickMsg(RetryButtonClickMsg msg)
    {
        Message.Send(new RequestGameStateDialogExitMsg(GameStateManager.State.GameEnd));
        StartCoroutine(RetryCoroutine());
    }

    void OnRestartButtonClickMsg(RestartButtonClickMsg msg)
    {
        //ChangeState to Intro.
        Message.Send(new GameStateChangeMsg(GameStateManager.State.Intro));
    }
    void OnGoBackLobbyButtonClickMsg(GoBackLobbyButtonClickMsg msg)
    {
        //ChangeState to Lobby.
        Message.Send(new GameStateChangeMsg(GameStateManager.State.Lobby));
    }
}
