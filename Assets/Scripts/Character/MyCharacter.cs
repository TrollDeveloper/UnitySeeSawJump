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
public class CharacterDownfallCompleteMsg : Message
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

    Sequence lastSequence;
    Coroutine stateEnterCoroutine = null;

    float controlDirection = 0f;

    GameContentModel contentModel;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        Message.AddListener<ResponseSeeSawPositionMsg>(OnResponseSeeSawPositionMsg);
    }

    // Start is called before the first frame update
    void Start()
    {
        contentModel = Model.First<GameContentModel>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.RocketJumping:
                transform.position += Vector3.up * Time.deltaTime * 30f;

                contentModel.characterHeight = transform.position.y;

                if (transform.position.y > contentModel.targetHeight)
                {
                    Message.Send(new GameStateChangeMsg(GameStateManager.State.Downfall));
                }
                break;
            case State.Downfall:
                transform.position -= Vector3.up * Time.deltaTime * 10f;
                transform.position += controlDirection * Vector3.right * 5f * Time.deltaTime;

                contentModel.characterHeight = transform.position.y;

                if (transform.position.y < 15f)
                {
                    Message.Send(new CharacterDownfallCompleteMsg());
                }
                break;
        }

        controlDirection = 0f;
    }

    public void ChangeState(State newState)
    {
        if (targetSeeSawSocket == null)
        {
            Message.Send<RequestSeeSawPositionMsg>(new RequestSeeSawPositionMsg());
        }
        gameObject.SetActive(true);

        //이전에 걸어둔 DoTween 있으면 제거.
        if (lastSequence != null)
        {
            lastSequence.Kill();
            lastSequence = null;
        }
        transform.DOKill();

        //이전에 걸어둔 코루틴 있으면 정지.
        if (stateEnterCoroutine != null)
        {
            StopCoroutine(stateEnterCoroutine);
        }
        stateEnterCoroutine = null;

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
                transform.localRotation = Quaternion.identity;
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
                lastSequence = DOTween.Sequence();

                lastSequence.Append(transform.DOMove(transform.position + Vector3.up * 3f, 1.5f).SetEase(Ease.OutCubic));

                var tween = transform.DOMove(targetSeeSawSocket.position, 0.5f);
                tween.onComplete = () => { Message.Send<CharacterLandingCompleteMsg>(new CharacterLandingCompleteMsg()); };
                lastSequence.AppendInterval(0.5f).Append(tween);

                break;
            case State.SeeSawJumpingWait:
                transform.position = targetSeeSawSocket.position;
                break;
            case State.SeeSawJumping:
                transform.DOMove(transform.position + Vector3.up * 15f, 1f).SetEase(Ease.OutCubic).onComplete =
                    () => { Message.Send<CharacterJumpingCompleteMsg>(new CharacterJumpingCompleteMsg()); };
                break;
            case State.SeeSawLandingWait:
                transform.position = targetSeeSawSocket.transform.position + Vector3.up * 15f;
                break;
            case State.SeeSawLanding:

                tween = transform.DOMove(targetSeeSawSocket.position, 0.5f);
                tween.onComplete = () => { Message.Send<CharacterLandingCompleteMsg>(new CharacterLandingCompleteMsg()); };
                lastSequence.AppendInterval(0.5f).Append(tween);

                break;
            case State.SeeSawLandingFail:
                transform.position = new Vector3(side == CharacterManager.CharacterSide.Left ? -3f : 3f, transform.position.y, transform.position.z);
                tween = transform.DOMove(targetSeeSawSocket.position, 0.5f);
                tween.onComplete = () => { Message.Send<CharacterLandingCompleteMsg>(new CharacterLandingCompleteMsg()); };
                lastSequence.AppendInterval(0.5f).Append(tween);
                break;
            case State.SeeSawLandingComplete:
                transform.parent = targetSeeSawSocket;
                transform.localPosition = Vector3.zero;
                break;
            case State.RocketJumping:
                transform.position = new Vector3(0f, 15f, transform.position.z);
                break;
            case State.DonwfallWait:
                stateEnterCoroutine = StartCoroutine(DownfallWaitCoroutine());
                break;
            case State.Downfall:
                Message.AddListener<MoveLastFingerPositionMsg>(OnMoveLastFingerPositionMsg);
                break;
            case State.Selecting:
                break;
            default:
                break;
        }
    }

    IEnumerator DownfallWaitCoroutine()
    {
        yield return new WaitForSeconds(1f);
        ChangeState(State.Downfall);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (state == State.Downfall && other.CompareTag("Item"))
        {
            var item = other.GetComponent<MyItem>();
            item?.OnHit();
        }
    }

    void OnResponseSeeSawPositionMsg(ResponseSeeSawPositionMsg msg)
    {
        targetSeeSawSocket = side == CharacterManager.CharacterSide.Left ? msg.leftSocket : msg.rightSocket;
    }

    void OnMoveLastFingerPositionMsg(MoveLastFingerPositionMsg msg)
    {
        if (state == State.Downfall)
        {
            //ControlType == MovePosition.
            if (true)
            {
                Vector3 worldPosition = Camera.main.ViewportToWorldPoint(msg.viewportPosition);

                //타겟 지점과 거리가 일정 이상일때.
                if (Mathf.Abs(transform.position.x - worldPosition.x) > 0.01f)
                {//이동방향 설정.
                    controlDirection = transform.position.x < worldPosition.x ? 1f : -1f;
                }
            }
            else
            {
            }
        }
    }
}