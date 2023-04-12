using System;

namespace Teuria;

public class Button : Control
{
    public Action? OnLeftClicked;
    public Action? OnRightClicked;
    public Action? OnMiddleClicked;


    public override void OnMouseEvent(MouseButton state)
    {
        switch (state) 
        {
        case MouseButton.LeftClicked:
            OnLeftClicked?.Invoke();
            break;
        case MouseButton.RightClicked:
            OnRightClicked?.Invoke();
            break;
        case MouseButton.MiddleClicked:
            OnMiddleClicked?.Invoke();
            break;
        }
    }
}