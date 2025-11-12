using System;

public sealed class TimerDataObject
{
    public string Title;
    public float TimeDuration;
    public Action ActionToExecute;
    public Action<string> TickTimeEvent;
    public bool IsNetworkGlobal;
}
