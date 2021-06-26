using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeControl;
using Character;
using UI;
using UI.Dialog;

namespace Content
{
    public class SetJumpingGageInfoMsg : Message
    {
        public float minNormal;
        public float maxNormal;
        public float minPerfect;
        public float maxPerfect;

        public SetJumpingGageInfoMsg(float minNormal, float maxNormal, float minPerfect, float maxPerfect)
        {
            this.minNormal = minNormal;
            this.maxNormal = maxNormal;
            this.minPerfect = minPerfect;
            this.maxPerfect = maxPerfect;
        }
    }

    public class UpdateJumpingGageMsg : Message
    {
        public float gage;

        public UpdateJumpingGageMsg(float gage)
        {
            this.gage = gage;
        }
    }

    public class JumpingContent : InGameContentBase
    {
        const int MaxJumpCount = 5;
        float jumpGage = 0f;

        float currentPower = 0f;

        bool isTouchDown = false;

        public override void Enter()
        {
            base.Enter();
            isTouchDown = false;
            //Character Position.
            Message.Send(new CharacterChangeStateFromGameStateMsg(GameStateManager.State.Jumping));
            //Set Camera Lobby Mode.
            Message.Send(new CameraStateChangeMsg(CameraController.State.SeeSaw));

            //Msg AddListener.
            Message.AddListener<CharacterJumpingCompleteMsg>(OnCharacterJumpingCompleteMsg);
            Message.AddListener<CharacterLandingCompleteMsg>(OnCharacterLandingCompleteMsg);

            StartCoroutine(JumpingStartCoroutine());
        }

        public override void Exit()
        {
            base.Exit();
            //Jumping UI Off.
            UIManager.Instance.RequestDialogExit<JumpingDialog>();
            UIManager.Instance.RequestDialogExit<EnergyDialog>();

            //Msg RemoveListener.
            MessageHelper.RemoveListenerEndFrame<CharacterJumpingCompleteMsg>(OnCharacterJumpingCompleteMsg);
            MessageHelper.RemoveListenerEndFrame<CharacterLandingCompleteMsg>(OnCharacterLandingCompleteMsg);
            MessageHelper.RemoveListenerEndFrame<TouchDownMsg>(OnTouchDownMsg);

            StopAllCoroutines();
        }

        private void Update()
        {
            if (isActive == false)
            {
                return;
            }
        }


        public IEnumerator JumpingStartCoroutine()
        {
            //FadeIn.
            Message.Send<CameraFadeInMsg>(new CameraFadeInMsg(0.5f));

            yield return new WaitForSeconds(1f);
            //Jumping UI ON
            UIManager.Instance.RequestDialogEnter<JumpingDialog>();
            Message.AddListener<TouchDownMsg>(OnTouchDownMsg);

            StartCoroutine("JumpGageUpdateCoroutine");
        }

        public IEnumerator JumpingEndCoroutine()
        {
            //FadeOut.
            Message.Send<CameraFadeOutMsg>(new CameraFadeOutMsg(0.5f));
            yield return new WaitForSeconds(1f);
            Message.Send(new GameStateChangeMsg(GameStateManager.State.Rocket));
        }

        IEnumerator JumpGageUpdateCoroutine()
        {
            Message.Send(new SetJumpingGageInfoMsg(0.6f, 1.0f, 0.8f, 0.9f));

            jumpGage = 0f;
            int jumpCount = 0;

            //테스트용 임시 UI 링크.
            while (true)
            {
                yield return null;

                jumpGage += Time.deltaTime;
                if (jumpGage > 1f)
                {
                    jumpGage -= 1f;
                    if (++jumpCount > MaxJumpCount)
                    {
                        break;
                    }
                }

                Message.Send(new UpdateJumpingGageMsg(jumpGage));
            }

            OnLandingAction();
        }

        void OnTouchDownMsg(TouchDownMsg msg)
        {
            OnLandingAction();
        }

        void OnLandingAction()
        {
            if (isTouchDown)
            {
                return;
            }

            isTouchDown = true;
            StopCoroutine("JumpGageUpdateCoroutine");
            //Hide Jump UI.
            UIManager.Instance.RequestDialogExit<JumpingDialog>();
            UIManager.Instance.RequestDialogExit<EnergyDialog>();

            //Calculate Score.
            var contentModel = Model.First<GameContentModel>();
            currentPower = (float) contentModel.curEnergy / contentModel.totalEnergy;

            if (jumpGage > 0.8f && jumpGage < 0.9f)
            {
                currentPower *= 1.2f;
            }
            else if (jumpGage > 0.6f && jumpGage <= 1f)
            {
                currentPower *= 1.0f;
            }
            else
            {
                currentPower *= 0.6f;
            }

            //Set Character Landing.
            Message.Send(new CharacterChangeStateMsg(
                currentPower >= 0.8f ? MyCharacter.State.SeeSawLanding : MyCharacter.State.SeeSawLandingFail, true));
        }

        void OnCharacterLandingCompleteMsg(CharacterLandingCompleteMsg msg)
        {
            //if Score is low then Change state GameEnd.
            if (currentPower < 0.8f)
            {
                Message.Send(new GameStateChangeMsg(GameStateManager.State.GameEnd));
            }
            else
            {
                var contentModel = Model.First<GameContentModel>();
                contentModel.targetHeight += 10f;
                //else ChangeCharacter and Jumping.
                Message.Send(new CharacterJumpingSwapMsg());
            }
        }

        void OnCharacterJumpingCompleteMsg(CharacterJumpingCompleteMsg msg)
        {
            StartCoroutine(JumpingEndCoroutine());
        }
    }
}