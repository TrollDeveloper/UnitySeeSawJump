using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeControl;

public class JumpingContent : InGameContentBase
{
    const int MaxJumpCount = 5;
    float jumpGage = 0f;

    public override void Enter()
    {
        base.Enter();
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
        Message.Send(new RequestGameStateDialogExitMsg(GameStateManager.State.Jumping));

        //Msg RemoveListener.
        MessageHelper.RemoveListenerEndFrame<CharacterJumpingCompleteMsg>(OnCharacterJumpingCompleteMsg);
        MessageHelper.RemoveListenerEndFrame<CharacterLandingCompleteMsg>(OnCharacterLandingCompleteMsg);
        MessageHelper.RemoveListenerEndFrame<TouchDownMsg>(OnTouchDownMsg);

        StopAllCoroutines();
    }

    private void Update()
    {
        if (isActive == false) { return; }

    }


    public IEnumerator JumpingStartCoroutine()
    {
        //FadeIn.
        Message.Send<CameraFadeInMsg>(new CameraFadeInMsg(0.5f));

        yield return new WaitForSeconds(1f);
        //Jumping UI ON
        Message.Send(new RequestGameStateDialogEnterMsg(GameStateManager.State.Jumping));
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
        jumpGage = 0f;
        int jumpCount = 0;

        //�׽�Ʈ�� �ӽ� UI ��ũ.
        Text text = GameObject.Find("GageText").GetComponent<Text>();
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

            text.text = (Mathf.Clamp01(jumpGage) * 100f).ToString("n00") + "%";
        }
        OnLandingAction();
    }

    void OnTouchDownMsg(TouchDownMsg msg)
    {
        OnLandingAction();
    }

    void OnLandingAction()
    {
        StopCoroutine("JumpGageUpdateCoroutine");
        //Hide Jump UI.
        Message.Send(new RequestGameStateDialogExitMsg(GameStateManager.State.Jumping));

        //Calculate Score.

        //Set Character Landing.
        Message.Send(new CharacterChangeStateMsg(MyCharacter.State.SeeSawLanding, true));
    }

    void OnCharacterLandingCompleteMsg(CharacterLandingCompleteMsg msg)
    {
        //if Score is low then Change state GameEnd.
        if (jumpGage < 0.6f)
        {
            Message.Send(new GameStateChangeMsg(GameStateManager.State.GameEnd));
        }
        else
        {
            //else ChangeCharacter and Jumping.
            Message.Send(new CharacterJumpingSwapMsg());
        }
    }

    void OnCharacterJumpingCompleteMsg(CharacterJumpingCompleteMsg msg)
    {
        StartCoroutine(JumpingEndCoroutine());
    }
}
