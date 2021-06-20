using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

namespace Item
{
    public class CleanUpAllItemMsg : Message
    {
    }

    public class MyItem : MonoBehaviour
    {
        private void OnEnable()
        {
            Message.AddListener<CleanUpAllItemMsg>(OnCleanUpAllItemMsg);
        }

        private void OnDisable()
        {
            MessageHelper.RemoveListenerEndFrame<CleanUpAllItemMsg>(OnCleanUpAllItemMsg);
        }

        void OnCleanUpAllItemMsg(CleanUpAllItemMsg msg)
        {
            gameObject.SetActive(false);
        }

        public virtual void OnHit()
        {
            gameObject.SetActive(false);
            Model.First<GameContentModel>().getItem++;
        }
    }
}