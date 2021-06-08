using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Com.LuisPedroFonseca.ProCamera2D;

public class CameraStateChangeMsg : Message
{
    public CameraController.State state;
    public CameraStateChangeMsg(CameraController.State newState)
    {
        state = newState;
    }
}

public class CameraFadeInMsg : Message
{
    public CameraFadeInMsg(float duration)
    {
        this.duration = duration;
    }
    public float duration;
}

public class CameraFadeOutMsg : Message
{
    public CameraFadeOutMsg(float duration)
    {
        this.duration = duration;
    }
    public float duration;
}

public class CameraController : MonoBehaviour
{
    public enum State
    {
        SeeSaw,
        Rocket,
        Downfall,
    }
    State state;

    MyCharacter targetCharacter;
    ProCamera2DTransitionsFX camTransition;
    ProCamera2D proCamera;

    // Start is called before the first frame update
    void Awake()
    {
        proCamera = GetComponent<ProCamera2D>();
        camTransition = GetComponent<ProCamera2DTransitionsFX>();

        Message.AddListener<CameraStateChangeMsg>(OnCameraChangeStateMsg);
        Message.AddListener<CameraFadeInMsg>(OnCameraFadeInMsg);
        Message.AddListener<CameraFadeOutMsg>(OnCameraFadeOutMsg);
        Message.AddListener<CharacterSideChangedMsg>(OnCharacterSideChangedMsg);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnDestroy()
    {
        MessageHelper.RemoveListenerEndFrame<CameraStateChangeMsg>(OnCameraChangeStateMsg);
        MessageHelper.RemoveListenerEndFrame<CameraFadeInMsg>(OnCameraFadeInMsg);
        MessageHelper.RemoveListenerEndFrame<CameraFadeOutMsg>(OnCameraFadeOutMsg);
    }

    public void ChangeState(State newState)
    {
        state = newState;
        switch (newState)
        {
            case State.SeeSaw:
                proCamera.FollowHorizontal = false;
                proCamera.FollowVertical = false;
                proCamera.MoveCameraInstantlyToPosition(new Vector2(0f, 0f));
                break;
            case State.Rocket:
                proCamera.FollowHorizontal = true;
                proCamera.FollowVertical = true;
                proCamera.CenterOnTargets();
                break;
            case State.Downfall:
                proCamera.FollowHorizontal = true;
                proCamera.FollowVertical = true;
                break;
        }
    }

    void SetTargetCharacter(MyCharacter newTargetCharacter)
    {
        if (targetCharacter != null)
        {
            proCamera.RemoveCameraTarget(targetCharacter.transform);
        }
        proCamera.AddCameraTarget(newTargetCharacter.transform);
        targetCharacter = newTargetCharacter;
    }

    void OnCameraChangeStateMsg(CameraStateChangeMsg msg)
    {
        ChangeState(msg.state);
    }
    void OnCameraFadeInMsg(CameraFadeInMsg msg)
    {
        camTransition.TransitionEnter();
    }
    void OnCameraFadeOutMsg(CameraFadeOutMsg msg)
    {
        camTransition.TransitionExit();
    }
    void OnCharacterSideChangedMsg(CharacterSideChangedMsg msg)
    {
        SetTargetCharacter(msg.myCharacter);

    }
}
