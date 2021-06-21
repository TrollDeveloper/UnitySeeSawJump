using UnityEngine;
using CodeControl;
using Character;

namespace Item
{
    public class ItemMagnetic : MyItem
    {
        public override void OnHit()
        {
            base.OnHit();

            Message.Send(new CharacterActiveMagneticMsg());
        }
    }
}