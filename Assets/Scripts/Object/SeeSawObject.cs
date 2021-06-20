using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Character;

public class RequestSeeSawPositionMsg : Message
{
}

public class ResponseSeeSawPositionMsg : Message
{
    public ResponseSeeSawPositionMsg(Transform left, Transform right)
    {
        this.leftSocket = left;
        this.rightSocket = right;
    }

    public Transform leftSocket;
    public Transform rightSocket;
}

public class SeeSawObject : MonoBehaviour
{
    public Transform leftSocket;
    public Transform rightSocket;
    public Transform panel;

    CharacterManager.CharacterSide characterSide;

    private void Awake()
    {
        Message.AddListener<RequestSeeSawPositionMsg>(OnRequestSeeSawPositionMsg);
        Message.AddListener<CharacterSideChangedMsg>(OnCharacterSideChangedMsg);
    }

    private void OnDestroy()
    {
        MessageHelper.RemoveListenerEndFrame<RequestSeeSawPositionMsg>(OnRequestSeeSawPositionMsg);
        MessageHelper.RemoveListenerEndFrame<CharacterSideChangedMsg>(OnCharacterSideChangedMsg);
    }

    void OnRequestSeeSawPositionMsg(RequestSeeSawPositionMsg msg)
    {
        Message.Send(new ResponseSeeSawPositionMsg(leftSocket, rightSocket));
    }

    void OnCharacterSideChangedMsg(CharacterSideChangedMsg msg)
    {
        characterSide = msg.side;
        panel.localEulerAngles =
            new Vector3(0f, 0f, (characterSide == CharacterManager.CharacterSide.Left ? 1 : -1) * -10f);
    }
}