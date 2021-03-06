using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Item;
using Character;
using UI;
using UI.Dialog;

namespace Content
{
    public class IntroContent : InGameContentBase
    {
        public override void Enter()
        {
            base.Enter();
            //Reset Data.
            Model.First<GameContentModel>().Init();
            Message.Send(new CleanUpAllItemMsg());

            Message.Send(new CameraFadeInMsg(0f));
            //Character Set.
            Message.Send(new CharacterChangeStateFromGameStateMsg(GameStateManager.State.Intro));
            //Camera Set.
            Message.Send(new CameraStateChangeMsg(CameraController.State.SeeSaw));
            
            //ResetDialog ( When Restart ) 
            UIManager.Instance.ResetDialog();
            //Msg AddListener.
            Message.AddListener<CharacterJumpingCompleteMsg>(OnCharacterJumpingCompleteMsg);
            Message.AddListener<CharacterLandingCompleteMsg>(OnCharacterLandingCompleteMsg);
        }

        public override void Exit()
        {
            base.Exit();
            //Msg RemoveListener.
            Message.RemoveListener<CharacterJumpingCompleteMsg>(OnCharacterJumpingCompleteMsg);
            Message.RemoveListener<CharacterLandingCompleteMsg>(OnCharacterLandingCompleteMsg);

            StopAllCoroutines();
        }

        void OnCharacterLandingCompleteMsg(CharacterLandingCompleteMsg msg)
        {
            Message.Send(new CharacterJumpingSwapMsg());
        }

        void OnCharacterJumpingCompleteMsg(CharacterJumpingCompleteMsg msg)
        {
            StartCoroutine(ChangeStateCoroutine());
        }

        IEnumerator ChangeStateCoroutine()
        {
            //FadeOut.
            Message.Send<CameraFadeOutMsg>(new CameraFadeOutMsg(0.5f));
            yield return new WaitForSeconds(1f);
            //ChangeState.
            Message.Send(new GameStateChangeMsg(GameStateManager.State.Rocket));
        }
    }
}