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
        //Retry Timer ON.

        Message.AddListener<RetryButtonClickMsg>(OnRetryButtonClickMsg);
        Message.AddListener<RestartButtonClickMsg>(OnRestartButtonClickMsg);
        Message.AddListener<GoBackLobbyButtonClickMsg>(OnGoBackLobbyButtonClickMsg);
    }
    public override void Exit()
    {
        base.Exit();
        //GameEnd UI Off.

        MessageHelper.RemoveListenerEndFrame<RetryButtonClickMsg>(OnRetryButtonClickMsg);
        MessageHelper.RemoveListenerEndFrame<RestartButtonClickMsg>(OnRestartButtonClickMsg);
        MessageHelper.RemoveListenerEndFrame<GoBackLobbyButtonClickMsg>(OnGoBackLobbyButtonClickMsg);
    }
    IEnumerator RetryCoroutine()
    {
        //Score Rollback.
        //Fade out-> ChangeState to Rocket.
        yield return new WaitForSeconds(1f);
        Message.Send<GameStateChangeMsg>(new GameStateChangeMsg(GameStateManager.State.Rocket));
    }
    void OnRetryButtonClickMsg(RetryButtonClickMsg msg)
    {
        StartCoroutine(RetryCoroutine());
    }

    void OnRestartButtonClickMsg(RestartButtonClickMsg msg)
    {
        //ChangeState to Intro.
        Message.Send<GameStateChangeMsg>(new GameStateChangeMsg(GameStateManager.State.Intro));
    }
    void OnGoBackLobbyButtonClickMsg(GoBackLobbyButtonClickMsg msg)
    {
        //ChangeState to Lobby.
        Message.Send<GameStateChangeMsg>(new GameStateChangeMsg(GameStateManager.State.Lobby));
    }
}