using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeControl;

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
        jumpGage = 0f;
        int jumpCount = 0;

        //테스트용 임시 UI 링크.
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
        if (isTouchDown)
        {
            return;
        }

        isTouchDown = true;
        StopCoroutine("JumpGageUpdateCoroutine");
        //Hide Jump UI.
        UIManager.Instance.RequestDialogExit<JumpingDialog>();

        //Calculate Score.
        var contentModel = Model.First<GameContentModel>();
        currentPower = (float) contentModel.getItem / contentModel.totalItem;

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