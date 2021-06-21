using System;
using System.Collections;
using System.Collections.Generic;
using Item;
using UnityEngine;
using CodeControl;

namespace Character
{
    public class MagneticCollider : MonoBehaviour
    {
        private MyCharacter ownCharacter;
        private Coroutine activeCoroutine;
        private Collider sphereCollider;

        private void Awake()
        {
            sphereCollider = GetComponent<SphereCollider>();
            sphereCollider.enabled = false;
            Message.AddListener<CharacterSideChangedMsg>(OnCharacterSideChangedMsg);
            Message.AddListener<CharacterActiveMagneticMsg>(OnCharacterActiveMagneticMsg);
            Message.AddListener<CharacterDownfallCompleteMsg>(OnCharacterDownfallCompleteMsg);
        }

        private void OnDestroy()
        {
            Message.RemoveListener<CharacterSideChangedMsg>(OnCharacterSideChangedMsg);
            Message.RemoveListener<CharacterActiveMagneticMsg>(OnCharacterActiveMagneticMsg);
            Message.RemoveListener<CharacterDownfallCompleteMsg>(OnCharacterDownfallCompleteMsg);
        }

        public void Active()
        {
            sphereCollider.enabled = true;

            transform.parent = ownCharacter.transform;
            transform.localPosition = Vector3.zero;

            if (activeCoroutine != null)
            {
                StopCoroutine(activeCoroutine);
            }

            activeCoroutine = StartCoroutine(ActiveCoroutine());
        }

        public void DeActive()
        {
            if (activeCoroutine != null)
            {
                StopCoroutine(activeCoroutine);
            }

            transform.parent = null;
            sphereCollider.enabled = false;
            activeCoroutine = null;
        }

        IEnumerator ActiveCoroutine()
        {
            yield return new WaitForSeconds(3f);
            DeActive();
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Item"))
            {
                other.GetComponent<MyItem>()?.OnMagnetic(ownCharacter.transform);
            }
        }

        void OnCharacterSideChangedMsg(CharacterSideChangedMsg msg)
        {
            ownCharacter = msg.myCharacter;
        }

        void OnCharacterActiveMagneticMsg(CharacterActiveMagneticMsg msg)
        {
            Active();
        }

        void OnCharacterDownfallCompleteMsg(CharacterDownfallCompleteMsg msg)
        {
            DeActive();
        }
    }
}