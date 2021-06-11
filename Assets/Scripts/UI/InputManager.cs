using CodeControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TouchDownMsg : Message
{
    public Vector2 viewportPosition;
    public TouchDownMsg(Vector2 viewportPosition)
    {
        this.viewportPosition = viewportPosition;
    }
}
public class LastFingerPositionMsg : Message
{
    public Vector2 viewportPosition;
    public LastFingerPositionMsg(Vector2 viewportPosition)
    {
        this.viewportPosition = viewportPosition;
    }
}
public class InputManager : MonoBehaviour
{
    List<int> fingerIDList = new List<int>();

    GraphicRaycaster graphicRaycaster;

    void Awake()
    {
        //������ Canvas�ϳ����̴� �׳� �޾ƿ���.
        graphicRaycaster = FindObjectOfType<GraphicRaycaster>();
    }

    void Update()
    {
        TouchInput();
    }
    void TouchInput()
    {
        float invScreenWidth = 1f / Screen.width;
        float invScreenHeight = 1f / Screen.height;

#if UNITY_EDITOR
        //�������Ͻ� ���콺 ��ǲ ����.
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointOverUI(Input.mousePosition) == false)
            {
                Message.Send(new TouchDownMsg(new Vector2(Input.mousePosition.x * invScreenWidth, Input.mousePosition.y * invScreenHeight)));
            }
        }
        if (Input.GetMouseButton(0))
        {
            Message.Send(new LastFingerPositionMsg(new Vector2(Input.mousePosition.x * invScreenWidth, Input.mousePosition.y * invScreenHeight)));
        }
#endif
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Began)
            {
                //Button Down�� UI���� �Ͼ�� �ʾ��� ��츸 ����.
                if (IsPointOverUI(touch.position) == false)
                {
                    fingerIDList.Add(touch.fingerId);
                    Message.Send(new TouchDownMsg(new Vector2(touch.position.x * invScreenWidth, touch.position.y * invScreenHeight)));
                }
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                fingerIDList.Remove(touch.fingerId);
            }
            else if (fingerIDList.Count != 0 && touch.fingerId == fingerIDList[fingerIDList.Count - 1])
            {// ���� ��ġ�� ������ ��ġ�� ��ġ ���� ó��.
                Message.Send(new LastFingerPositionMsg(new Vector2(touch.position.x * invScreenWidth, touch.position.y * invScreenHeight)));
            }
        }
    }

    // �ش� ��ũ�� ��ġ�� UI�� �ִ��� �˻�.
    bool IsPointOverUI(Vector2 screenPosition)
    {
        if (graphicRaycaster != null)
        {
            PointerEventData ped = new PointerEventData(null);
            ped.position = screenPosition;
            List<RaycastResult> result = new List<RaycastResult>();
            graphicRaycaster.Raycast(ped, result);
            if (result.Count != 0)
            {
                return true;
            }
        }
        return false;
    }
}