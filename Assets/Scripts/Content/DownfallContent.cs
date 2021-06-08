using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class DownfallContent : InGameContentBase
{
    public override void Enter()
    {
        base.Enter();
        //Set Character Downfall Position.
        Message.Send(new CharacterChangeStateFromGameStateMsg(GameStateManager.State.Downfall));
        //Camera State Change.
        Message.Send(new CameraStateChangeMsg(CameraController.State.Downfall));
        //StartCoroutine Character Downfall StartDelay.
        StartCoroutine(DownfallStartCoroutine());
    }

    public override void Exit()
    {
        base.Exit();
        //Background Display Turn Off.
        //Control UI Off.
    }

    IEnumerator DownfallStartCoroutine()
    {
        yield return new WaitForSeconds(1f);
        Message.Send(new CharacterChangeStateMsg(MyCharacter.State.Downfall));

        //Control UI ON.
    }

    void OnCharacterDownfallCompleteMsg()
    {
        StartCoroutine(DownfallEndCoroutine());
    }

    IEnumerator DownfallEndCoroutine()
    {
        //FadeOut.
        Message.Send<CameraFadeOutMsg>(new CameraFadeOutMsg(0.5f));
        yield return new WaitForSeconds(1f);
        //ChagneState -> Jumping.
        Message.Send(new GameStateChangeMsg(GameStateManager.State.Jumping));
    }
}
