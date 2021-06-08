using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using DG.Tweening;
public class CharacterJumpingCompleteMsg : Message
{

}
public class CharacterLandingCompleteMsg : Message
{

}
public class CharacterRocketCompleteMsg : Message
{

}
public partial class MyCharacter : MonoBehaviour
{
    public enum State
    {
        None,
        IntroWait,
        IntroJumping,
        SeeSawJumpingWait,
        SeeSawJumping,
        SeeSawLandingWait,
        SeeSawLanding,
        SeeSawLandingFail,
        SeeSawLandingComplete,
        RocketJumping,
        DonwfallWait,
        Downfall,
        Selecting,
    }
    State state = State.None;

    CharacterManager.CharacterSide side;
    public CharacterManager.CharacterSide Side
    {
        get { return side; }
        set
        {
            side = value;
            Message.Send<RequestSeeSawPositionMsg>(new RequestSeeSawPositionMsg());
        }
    }

    Animator animator;

    Transform targetSeeSawSocket = null;

    Sequence seq;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        Message.AddListener<ResponseSeeSawPositionMsg>(OnResponseSeeSawPositionMsg);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.None:
                break;
            case State.IntroWait:
                break;
            case State.IntroJumping:
                break;
            case State.SeeSawJumpingWait:
                break;
            case State.SeeSawJumping:
                break;
            case State.SeeSawLandingWait:
                break;
            case State.SeeSawLanding:
                break;
            case State.SeeSawLandingFail:
                break;
            case State.RocketJumping:
                transform.position += Vector3.up * Time.deltaTime;
                break;
            case State.DonwfallWait:
                break;
            case State.Downfall:
                break;
        }
    }

    public void ChangeState(State newState)
    {
        if (targetSeeSawSocket == null)
        {
            Message.Send<RequestSeeSawPositionMsg>(new RequestSeeSawPositionMsg());
        }
        gameObject.SetActive(true);

        if (seq != null)
        {
            seq.Kill();
            seq = null;
        }
        transform.DOKill();
        switch (state)
        {
            case State.None:
                break;
            case State.IntroWait:
                break;
            case State.IntroJumping:
                break;
            case State.SeeSawJumpingWait:
                break;
            case State.SeeSawJumping:
                break;
            case State.SeeSawLandingWait:
                break;
            case State.SeeSawLanding:
                break;
            case State.SeeSawLandingFail:
                break;
            case State.SeeSawLandingComplete:
                transform.parent = null;
                break;
            case State.RocketJumping:
                break;
            case State.DonwfallWait:
                break;
            case State.Downfall:
                break;
            case State.Selecting:
                break;
            default:
                break;
        }

        state = newState;

        switch (state)
        {
            case State.None:
                break;
            case State.IntroWait:
                transform.position = targetSeeSawSocket.position;
                break;
            case State.IntroJumping:
                transform.position = targetSeeSawSocket.position;
                seq = DOTween.Sequence();
                
                seq.Append(transform.DOMove(transform.position + Vector3.up * 3f, 1.5f).SetEase(Ease.OutCubic));

                var tween = transform.DOMove(targetSeeSawSocket.position, 0.5f);
                tween.onComplete = () => { Message.Send<CharacterLandingCompleteMsg>(new CharacterLandingCompleteMsg()); };
                seq.AppendInterval(0.5f).Append(tween);

                break;
            case State.SeeSawJumpingWait:
                transform.position = targetSeeSawSocket.position;
                break;
            case State.SeeSawJumping:
                transform.DOMove(transform.position + Vector3.up * 15f, 1f).SetEase(Ease.OutCubic).onComplete =
                    () => { Message.Send<CharacterJumpingCompleteMsg>(new CharacterJumpingCompleteMsg()); };
                break;
            case State.SeeSawLandingWait:
                break;
            case State.SeeSawLanding:
                break;
            case State.SeeSawLandingFail:
                break;
            case State.SeeSawLandingComplete:
                transform.parent = targetSeeSawSocket;
                transform.localPosition = Vector3.zero;
                break;
            case State.RocketJumping:
                break;
            case State.DonwfallWait:
                break;
            case State.Downfall:
                break;
            case State.Selecting:
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {

    }

    void OnResponseSeeSawPositionMsg(ResponseSeeSawPositionMsg msg)
    {
        targetSeeSawSocket = side == CharacterManager.CharacterSide.Left ? msg.leftSocket : msg.rightSocket;
    }
}