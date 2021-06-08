using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class GameStateChangeMsg : Message
{
    public GameStateManager.State state;
    public GameStateChangeMsg(GameStateManager.State newState)
    {
        state = newState;
    }
}

public class GameStateManager : MonoBehaviour
{
    public enum State
    {
        Lobby,
        Intro,
        Rocket,
        Downfall,
        Jumping,
        GameEnd,
    }

    State currentState = State.Lobby;
    Dictionary<State, InGameContentBase> contentList = new Dictionary<State, InGameContentBase>();
    InGameContentBase currentContent;

    // Start is called before the first frame update
    void Awake()
    {
        InitContentList();
        Message.AddListener<GameStateChangeMsg>(OnGameStateChangeMsg);
    }
    void Start()
    {
        ChangeState(State.Lobby);
    }
    private void OnDestroy()
    {
        MessageHelper.RemoveListenerEndFrame<GameStateChangeMsg>(OnGameStateChangeMsg);
    }

    void InitContentList()
    {
        AddContentList<LobbyContent>(State.Lobby);
        AddContentList<IntroContent>(State.Intro);
        AddContentList<RocketContent>(State.Rocket);
        AddContentList<DownfallContent>(State.Downfall);
        AddContentList<JumpingContent>(State.Jumping);
        AddContentList<GameEndContent>(State.GameEnd);
    }
    void AddContentList<T>(State state) where T : InGameContentBase
    {
        if (contentList.ContainsKey(state) == false)
        {
            GameObject newObject = new GameObject();
            newObject.transform.parent = transform;
            newObject.name = state.ToString();

            T newContent = newObject.AddComponent<T>();
            contentList.Add(state, newContent);

            DebugLog.Log("Add Content State : " + state);
        }
    }

    public void ChangeState(State newState)
    {
        if (contentList.ContainsKey(newState) == false)
        {
            InitContentList();
        }
        if (currentContent != null)
        {
            currentContent.Exit();
        }
        currentState = newState;
        currentContent = contentList[newState];

        currentContent.Enter();
    }

    void OnGameStateChangeMsg(GameStateChangeMsg msg)
    {
        ChangeState(msg.state);
    }
}
