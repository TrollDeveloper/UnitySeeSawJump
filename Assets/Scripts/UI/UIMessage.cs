using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;


public class StartButtonClickMsg : VoidMessageBase
{
    public override void Send()
    {
        Message.Send(this);
    }
}
public class JumpButtonClickMsg : VoidMessageBase
{
    public override void Send()
    {
        Message.Send(this);
    }
}

public class RetryButtonClickMsg : VoidMessageBase
{
    public override void Send()
    {
        Message.Send(this);
    }
}
public class RestartButtonClickMsg : VoidMessageBase
{
    public override void Send()
    {
        Message.Send(this);
    }
}
public class GoBackLobbyButtonClickMsg : VoidMessageBase
{
    public override void Send()
    {
        Message.Send(this);
    }
}