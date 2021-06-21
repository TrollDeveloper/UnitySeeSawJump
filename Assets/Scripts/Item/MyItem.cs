using System;
using System.Collections;
using System.Collections.Generic;
using Character;
using UnityEngine;
using CodeControl;
using UnityEngine.Serialization;

namespace Item
{
    public class CleanUpAllItemMsg : Message
    {
    }

    public class MyItem : MonoBehaviour
    {
        public int energy;

        bool isFollowing = false;

        public void Init()
        {
            isFollowing = false;
            Model.First<GameContentModel>().totalEnergy += energy;
            MessageHelper.AddListenerEndFrame<CleanUpAllItemMsg>(OnCleanUpAllItemMsg);
        }

        private void OnDisable()
        {
            MessageHelper.RemoveListenerEndFrame<CleanUpAllItemMsg>(OnCleanUpAllItemMsg);
        }

        IEnumerator FollowingCoroutine(Transform followingTarget)
        {
            float speed = 0f;
            while (true)
            {
                yield return null;
                if (followingTarget != null)
                {
                    speed += Time.deltaTime * 30f;

                    Vector3 position = transform.position;

                    float moveDistance = speed * Time.deltaTime;
                    Vector3 direction = (followingTarget.position - position);
                    float distance = direction.magnitude;
                    if (moveDistance < distance)
                    {
                        position += direction / distance * moveDistance;
                    }
                    else
                    {
                        position = followingTarget.position;
                    }

                    transform.position = position;
                }
            }
        }

        void OnCleanUpAllItemMsg(CleanUpAllItemMsg msg)
        {
            gameObject.SetActive(false);
        }

        public void OnMagnetic(Transform target)
        {
            if (isFollowing == false)
            {
                isFollowing = true;
                StartCoroutine(FollowingCoroutine(target));
            }
        }

        public virtual void OnHit()
        {
            gameObject.SetActive(false);
            Model.First<GameContentModel>().curEnergy += energy;
        }
    }
}