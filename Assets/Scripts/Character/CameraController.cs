using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Com.LuisPedroFonseca.ProCamera2D;
using DG.Tweening;
using Character;
using Item;

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
    public float duration;

    public CameraFadeInMsg(float duration)
    {
        this.duration = duration;
    }
}

public class CameraFadeOutMsg : Message
{
    public float duration;

    public CameraFadeOutMsg(float duration)
    {
        this.duration = duration;
    }
}

public class CameraShakeMsg : Message
{
    public CameraController.ShakeType type;

    public CameraShakeMsg(CameraController.ShakeType type)
    {
        this.type = type;
    }
}

public class CameraController : MonoBehaviour
{
    public enum ShakeType : int
    {
        Landing = 0,
    }

    public enum State
    {
        SeeSaw,
        Rocket,
        Downfall,
    }

    private State state;

    private MyCharacter targetCharacter;
    private ProCamera2DTransitionsFX camTransition;
    private ProCamera2D proCamera;
    private ProCamera2DNumericBoundaries boundaries;
    private ProCamera2DShake cameraShake;
    private Tweener offsetTweener;

    // Start is called before the first frame update
    void Awake()
    {
        proCamera = GetComponent<ProCamera2D>();
        camTransition = GetComponent<ProCamera2DTransitionsFX>();
        boundaries = GetComponent<ProCamera2DNumericBoundaries>();
        boundaries.enabled = false;
        cameraShake = GetComponent<ProCamera2DShake>();

        Message.AddListener<CameraStateChangeMsg>(OnCameraChangeStateMsg);
        Message.AddListener<CameraFadeInMsg>(OnCameraFadeInMsg);
        Message.AddListener<CameraFadeOutMsg>(OnCameraFadeOutMsg);
        Message.AddListener<CameraShakeMsg>(OnCameraShakeMsg);
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
        cameraShake.StopShaking();
        switch (state)
        {
            case State.Downfall:
                offsetTweener?.Kill();
                offsetTweener = null;
                boundaries.enabled = false;
                proCamera.OffsetY = 0f;
                break;
        }

        state = newState;
        switch (newState)
        {
            case State.SeeSaw:
                proCamera.FollowHorizontal = false;
                proCamera.FollowVertical = false;
                proCamera.MoveCameraInstantlyToPosition(new Vector2(0f, 0f));
                break;
            case State.Rocket:
                proCamera.FollowVertical = true;
                proCamera.CenterOnTargets();
                proCamera.OffsetY = 0f;
                break;
            case State.Downfall:
                proCamera.FollowVertical = true;
                boundaries.enabled = true;
                offsetTweener = DOTween.To(x => proCamera.OffsetY = x, 0f, -0.8f, 1f).SetEase(Ease.Linear).SetDelay(1f);
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

    public void ShakeCamera(ShakeType shakeType)
    {
        cameraShake.Shake((int) shakeType);
    }

    void OnCameraShakeMsg(CameraShakeMsg msg)
    {
        ShakeCamera(msg.type);
    }

    void OnCameraChangeStateMsg(CameraStateChangeMsg msg)
    {
        ChangeState(msg.state);
    }

    void OnCameraFadeInMsg(CameraFadeInMsg msg)
    {
        camTransition.DurationEnter = msg.duration;
        camTransition.TransitionEnter();
    }

    void OnCameraFadeOutMsg(CameraFadeOutMsg msg)
    {
        camTransition.DurationExit = msg.duration;
        camTransition.TransitionExit();
    }

    void OnCharacterSideChangedMsg(CharacterSideChangedMsg msg)
    {
        SetTargetCharacter(msg.myCharacter);
    }
}