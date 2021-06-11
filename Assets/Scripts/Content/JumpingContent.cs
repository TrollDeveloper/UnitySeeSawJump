using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class JumpingContent : InGameContentBase
{
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
        Message.AddListener<TouchDownMsg>(OnTouchDownMsg);

    }

    public IEnumerator JumpingEndCoroutine()
    {
        //FadeOut.
        Message.Send<CameraFadeOutMsg>(new CameraFadeOutMsg(0.5f));
        yield return new WaitForSeconds(1f);
        Message.Send(new GameStateChangeMsg(GameStateManager.State.Rocket));
    }

    void OnTouchDownMsg(TouchDownMsg msg)
    {
        OnLandingAction();
    }

    void OnLandingAction()
    {
        //Hide Jump UI.
        //Calculate Score.

        //Set Character Landing.
        Message.Send(new CharacterChangeStateMsg(MyCharacter.State.SeeSawLanding, true));
    }

    void OnCharacterLandingCompleteMsg(CharacterLandingCompleteMsg msg)
    {
        //if Score is low then Change state GameEnd.
        if (false)
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
