using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Item;
using Content;
namespace Character
{
    public class CharacterChangeStateFromGameStateMsg : Message
    {
        public GameStateManager.State state;

        public CharacterChangeStateFromGameStateMsg(GameStateManager.State newState)
        {
            state = newState;
        }
    }

    public class CharacterChangeStateMsg : Message
    {
        public bool isCurrentCharacter;
        public MyCharacter.State state;

        public CharacterChangeStateMsg(MyCharacter.State newState, bool newIsCurrentCharacter = true)
        {
            state = newState;
            isCurrentCharacter = newIsCurrentCharacter;
        }
    }

    public class CharacterJumpingSwapMsg : Message
    {
    }

    public class CharacterSideChangedMsg : Message
    {
        public CharacterSideChangedMsg(MyCharacter myCharacter, CharacterManager.CharacterSide side)
        {
            this.myCharacter = myCharacter;
            this.side = side;
        }

        public MyCharacter myCharacter;
        public CharacterManager.CharacterSide side;
    }

    public class CharacterManager : MonoBehaviour
    {
        public enum CharacterSide : int
        {
            Left = 0,
            Right = 1,
        }

        CharacterSide curSide = CharacterSide.Left;
        MyCharacter[] curCharacterArray = new MyCharacter[2];

        List<MyCharacter> characterList = new List<MyCharacter>();
        public List<int> characterPrefabIDList = new List<int>();

        // Start is called before the first frame update
        void Awake()
        {
            for (int i = 0; i < characterPrefabIDList.Count; i++)
            {
                var newCharacter = SpawnCharacter(characterPrefabIDList[i]);
                if (newCharacter != null)
                {
                    characterList.Add(newCharacter);
                }
            }

            SetCharacter(characterList[0], CharacterSide.Left);
            SetCharacter(characterList[1], CharacterSide.Right);

            Message.AddListener<CharacterChangeStateFromGameStateMsg>(OnCharacterChangeStateFromGameStateMsg);
            Message.AddListener<CharacterChangeStateMsg>(OnCharacterChangeStateMsg);
            Message.AddListener<CharacterJumpingSwapMsg>(OnCharacterJumpingSwapMsg);
        }

        private void OnDestroy()
        {
            MessageHelper.RemoveListenerEndFrame<CharacterChangeStateFromGameStateMsg>(
                OnCharacterChangeStateFromGameStateMsg);
            MessageHelper.RemoveListenerEndFrame<CharacterChangeStateMsg>(OnCharacterChangeStateMsg);
            MessageHelper.RemoveListenerEndFrame<CharacterJumpingSwapMsg>(OnCharacterJumpingSwapMsg);
        }

        MyCharacter SpawnCharacter(int id)
        {
            //ObjectPoolManager.Instance.SpawnObject(PrefabManager.Instance.GetPrefab(id));

            var prefab = PrefabManager.Instance.GetPrefab(id);
            if (prefab == null)
            {
                return null;
            }

            var newObj = Instantiate(prefab);
            MyCharacter newCharacter = newObj.GetComponent<MyCharacter>();
            if (newCharacter == null)
            {
                Destroy(newObj);
                DebugLog.LogError("Spawn Character Error : " + id);
                return null;
            }

            newObj.SetActive(false);

            return newCharacter;
        }

        public void SetCharacter(MyCharacter newCharacter, CharacterSide side)
        {
            if (GetCharacter(side) != null)
            {
                //DestroyCharacter.
            }

            newCharacter.gameObject.SetActive(true);
            newCharacter.Side = side;
            curCharacterArray[(int) side] = newCharacter;
        }

        public MyCharacter GetCharacter(CharacterSide side)
        {
            return curCharacterArray[(int) side];
        }

        public CharacterSide GetOppositeSide(CharacterSide side)
        {
            return (CharacterSide) (side == 0 ? 1 : 0);
        }

        void SetCharacterSide(CharacterSide side)
        {
            curSide = side;
            Message.Send<CharacterSideChangedMsg>(new CharacterSideChangedMsg(GetCharacter(curSide), curSide));
        }

        public void OnChangeGameState(GameStateManager.State state)
        {
            switch (state)
            {
                case GameStateManager.State.Lobby:
                    SetCharacterSide(CharacterSide.Left);
                    GetCharacter(CharacterSide.Left).ChangeState(MyCharacter.State.IntroWait);
                    GetCharacter(CharacterSide.Right).ChangeState(MyCharacter.State.SeeSawJumpingWait);
                    break;
                case GameStateManager.State.Intro:
                    SetCharacterSide(CharacterSide.Left);
                    GetCharacter(CharacterSide.Left).ChangeState(MyCharacter.State.IntroJumping);
                    GetCharacter(CharacterSide.Right).ChangeState(MyCharacter.State.SeeSawJumpingWait);
                    break;
                case GameStateManager.State.Rocket:
                    GetCharacter(curSide).ChangeState(MyCharacter.State.RocketJumping);
                    GetCharacter(GetOppositeSide(curSide)).gameObject.SetActive(false);
                    break;
                case GameStateManager.State.Downfall:
                    GetCharacter(curSide).ChangeState(MyCharacter.State.DonwfallWait);
                    GetCharacter(GetOppositeSide(curSide)).gameObject.SetActive(false);
                    break;
                case GameStateManager.State.Jumping:
                    GetCharacter(curSide).ChangeState(MyCharacter.State.SeeSawLandingWait);
                    GetCharacter(GetOppositeSide(curSide)).ChangeState(MyCharacter.State.SeeSawJumpingWait);
                    break;
                case GameStateManager.State.GameEnd:
                    break;
                default:
                    break;
            }
        }

        public void OnCharacterChangeStateFromGameStateMsg(CharacterChangeStateFromGameStateMsg msg)
        {
            OnChangeGameState(msg.state);
        }

        public void OnCharacterChangeStateMsg(CharacterChangeStateMsg msg)
        {
            CharacterSide side = msg.isCurrentCharacter ? curSide : GetOppositeSide(curSide);
            GetCharacter(side).ChangeState(msg.state);
        }

        public void OnCharacterJumpingSwapMsg(CharacterJumpingSwapMsg msg)
        {
            GetCharacter(curSide).ChangeState(MyCharacter.State.SeeSawLandingComplete);
            GetCharacter(GetOppositeSide(curSide)).ChangeState(MyCharacter.State.SeeSawJumping);

            SetCharacterSide(GetOppositeSide(curSide));
        }
    }
}