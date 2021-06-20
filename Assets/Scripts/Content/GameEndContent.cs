using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using UI;
using UI.Dialog;

namespace Content
{
    public class GameEndContent : InGameContentBase
    {
        public override void Enter()
        {
            base.Enter();
            //GameEnd UI ON.
            UIManager.Instance.RequestDialogEnter<GameEndDialog>();
            UIManager.Instance.RequestDialogExit<PauseDialog>();
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
            UIManager.Instance.RequestDialogExit<GameEndDialog>();
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
            UIManager.Instance.RequestDialogExit<GameEndDialog>();
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
}