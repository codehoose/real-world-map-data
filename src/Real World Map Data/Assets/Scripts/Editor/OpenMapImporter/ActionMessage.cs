using System;

internal sealed class ActionMessage
{
    private readonly Action _action;

    public ActionMessage(Action action)
    {
        _action = action;
    }

    public void Invoke()
    {
        _action.Invoke();
    }
}