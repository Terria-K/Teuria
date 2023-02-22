#if ANDROID
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace Teuria;

public class TouchInput : BaseInput
{
    public TouchCollection CurrentState;
    public TouchCollection PreviousState;

    internal TouchInput() {}

    public override void Update()
    {
        PreviousState = CurrentState;
        CurrentState = TouchPanel.GetState();
    }


    public bool Pressed(Rectangle rectangle) 
    {
        return !Disabled && CheckTouch(rectangle, CurrentState);
        // return !Disabled && CurrentState.IsKeyDown(key);
    }

    public bool JustPressed(Rectangle rectangle) 
    {
        return !Disabled && CheckTouch(rectangle, CurrentState) && !CheckTouch(rectangle, PreviousState);
        // return !Disabled && CurrentState.IsKeyDown(key) && k!PreviousState.IsKeyDown(key);
    }

    public bool Released(Rectangle rectangle) 
    {
        return !Disabled && !CheckTouch(rectangle, CurrentState) && CheckTouch(rectangle, PreviousState);
        // return !Disabled && !CurrentState.IsKeyDown(key) && PreviousState.IsKeyDown(key);
    }

    // public int GetAxis(Keys neg, Keys pos) 
    // {
    //     if (Disabled) return 0;
    //     return Pressed(neg) 
    //         ? (Pressed(pos) ? 0 : -1) 
    //         : (Pressed(pos) ? 1 : 0);
    // }

    private bool CheckTouch(Rectangle target, TouchCollection touchCollection)
    {
        if (touchCollection.Count <= 0)
            return false;
        
        foreach (var touch in touchCollection)
        {
            if (target.Contains(touch.Position))
                return true;
        }
        return false;
    }
}
#endif