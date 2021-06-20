using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

namespace Content
{
    public class InGameContentBase : MonoBehaviour
    {
        protected bool isActive = false;

        public virtual void Enter()
        {
            isActive = true;
        }

        public virtual void Exit()
        {
            isActive = false;
        }
    }
}