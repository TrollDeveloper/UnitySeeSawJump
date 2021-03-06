using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using DG.Tweening;
using Item;
using Content;

namespace Character
{
    #region Message

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

    public class CharacterActiveMagneticMsg : Message
    {
    }

    public class CharacterActiveRocketMsg : Message
    {
    }

    #endregion

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

        float lastTouchPointX = -999f;

        GameContentModel contentModel;

        private bool downfallComplete = false;

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
                    if (transform.position.y > contentModel.targetHeight)
                    {
                        Message.Send(new GameStateChangeMsg(GameStateManager.State.Downfall));
                    }

                    break;
                case State.Downfall:
                    DownfallUpdate();
                    break;
            }

            contentModel.characterHeight = transform.position.y;
            lastTouchPointX = -999f;
        }

        void DownfallUpdate()
        {
            Vector3 position = transform.position;

            position -= Vector3.up * Time.deltaTime * 10f;
            if (position.y < 15f && downfallComplete == false)
            {
                downfallComplete = true;
                Message.Send(new CharacterDownfallCompleteMsg());
            }

            transform.position = position;

            if (lastTouchPointX == -999f)
            {
                return;
            }

            //ControlType == MovePosition.
            float controlDirection = 0;
            var controlType = Model.First<GameSettingModel>().controlType;
            if (controlType == GameSettingModel.ControlType.TargetPoint)
            {
                Vector3 worldPosition = Camera.main.ViewportToWorldPoint(new Vector3(lastTouchPointX, 0.5f, 10f));

                //???????? ?????? ????. ?????????? ?????????? ???????? ???? ???? ???? ???????? ???? ????.
                if (Mathf.Abs(transform.position.x - worldPosition.x) > 5f * Time.deltaTime)
                {
                    //???? ?????? ?????? ???? ???????? ???????? ????.      
                    controlDirection = transform.position.x - worldPosition.x < 0f ? 1f : -1f;
                }
                else
                {
                    position.x = worldPosition.x;
                    transform.position = position;
                }
            }
            else if (controlType == GameSettingModel.ControlType.Direction)
            {
                controlDirection = lastTouchPointX > 0.5 ? 1 : -1;
            }
            else
            {
                controlDirection = (lastTouchPointX - 0.5f) * 2f;
            }


            position.x += controlDirection * 5f * Time.deltaTime;
            position.x = Mathf.Clamp(position.x, -4.5f, 4.5f);
            transform.position = position;
        }

        public void ChangeState(State newState)
        {
            if (targetSeeSawSocket == null)
            {
                Message.Send<RequestSeeSawPositionMsg>(new RequestSeeSawPositionMsg());
            }

            gameObject.SetActive(true);

            //?????? ?????? DoTween ?????? ????.
            if (lastSequence != null)
            {
                lastSequence.Kill();
                lastSequence = null;
            }

            transform.DOKill();

            //?????? ?????? ?????? ?????? ????.
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

                    lastSequence.Append(transform.DOMove(transform.position + Vector3.up * 3f, 1.5f)
                        .SetEase(Ease.OutCubic));

                    var tween = transform.DOMove(targetSeeSawSocket.position, 0.3f);
                    tween.onComplete = () => { LandingComplete((true)); };
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
                    tween.onComplete = () => { LandingComplete((true)); };
                    lastSequence.AppendInterval(0.5f).Append(tween);

                    break;
                case State.SeeSawLandingFail:
                    float offset = side == CharacterManager.CharacterSide.Left ? -1.5f : 1.5f;
                    transform.position = new Vector3(transform.position.x + offset,
                        transform.position.y, transform.position.z);
                    tween = transform.DOMove(new Vector3(transform.position.x, -7f, transform.position.z), 0.5f);
                    tween.onComplete = () => { LandingComplete((false)); };
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
                    break;
                case State.Downfall:
                    downfallComplete = false;
                    Message.AddListener<MoveLastFingerPositionMsg>(OnMoveLastFingerPositionMsg);
                    break;
                case State.Selecting:
                    break;
                default:
                    break;
            }
        }

        public void LandingComplete(bool isSuccess)
        {
            Message.Send<CharacterLandingCompleteMsg>(new CharacterLandingCompleteMsg());
            if (isSuccess)
            {
                Message.Send(new CameraShakeMsg(CameraController.ShakeType.Landing));
            }
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
                lastTouchPointX = msg.viewportPosition.x;
            }
        }
    }
}