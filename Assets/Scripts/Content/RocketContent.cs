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
    public class RocketContent : InGameContentBase
    {
        public override void Enter()
        {
            base.Enter();
            //Item Info Generate.
            Model.First<GameContentModel>().Reset();
            Message.Send(new GenerateItemMsg());

            //Background Display ON.
            //Character Rocket Start.
            Message.Send(new CharacterChangeStateFromGameStateMsg(GameStateManager.State.Rocket));
            //Fade In.
            Message.Send<CameraFadeInMsg>(new CameraFadeInMsg(0.5f));
            //Set Camera State.
            Message.Send(new CameraStateChangeMsg(CameraController.State.Rocket));
            //Change UI.
            UIManager.Instance.RequestDialogEnter<HeightDialog>();
            UIManager.Instance.RequestDialogEnter<PauseDialog>();


            Message.AddListener<CharacterRocketCompleteMsg>(OnCharacterRocketCompleteMsg);
        }

        public override void Exit()
        {
            base.Exit();
            MessageHelper.RemoveListenerEndFrame<CharacterRocketCompleteMsg>(OnCharacterRocketCompleteMsg);
        }

        private void Update()
        {
            if (isActive == false)
            {
                return;
            }
        }

        void OnCharacterRocketCompleteMsg(CharacterRocketCompleteMsg msg)
        {
            //Change Game State -> Downfall.
            Message.Send(new GameStateChangeMsg(GameStateManager.State.Downfall));
        }
    }
}