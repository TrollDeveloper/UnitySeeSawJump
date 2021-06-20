using System.Collections;
using System.Collections.Generic;
using Beebyte.Obfuscator;
using UnityEngine;
using CodeControl;

namespace UI.Dialog
{
    [SkipRename]
    public class RetryButtonClickMsg : VoidMessageBase
    {
        public override void Send()
        {
            Message.Send(this);
        }
    }

    [SkipRename]
    public class RestartButtonClickMsg : VoidMessageBase
    {
        public override void Send()
        {
            Message.Send(this);
        }
    }

    [SkipRename]
    public class GoBackLobbyButtonClickMsg : VoidMessageBase
    {
        public override void Send()
        {
            Message.Send(this);
        }
    }

    public class GameEndDialog : DialogBase
    {
    }
}