using Microsoft.Xna.Framework.Input;

namespace Teuria;

public class MouseInput : BaseInput
{
    public MouseState PreviousState;
    public MouseState CurrentState;

    public override void Update()
    {
        PreviousState = CurrentState;
        CurrentState = Mouse.GetState();
    }

    public int Scroll => CurrentState.ScrollWheelValue - PreviousState.ScrollWheelValue;
    public int ScrollValue => CurrentState.ScrollWheelValue;


    public bool LeftClicked() => CurrentState.LeftButton == ButtonState.Pressed;
    public bool RightClicked() => CurrentState.RightButton == ButtonState.Pressed;
    public bool MiddleClicked() => CurrentState.MiddleButton == ButtonState.Pressed;

    public bool JustLeftClicked() => 
        CurrentState.LeftButton == ButtonState.Pressed && PreviousState.LeftButton == ButtonState.Released;
    public bool JustRightClicked() => 
        CurrentState.RightButton == ButtonState.Pressed && PreviousState.RightButton == ButtonState.Released;
    public bool JustMiddleClicked() => 
        CurrentState.MiddleButton == ButtonState.Pressed && PreviousState.MiddleButton == ButtonState.Released;
    public bool LeftReleased() => 
        CurrentState.LeftButton == ButtonState.Released && PreviousState.LeftButton == ButtonState.Pressed;
    public bool RightReleased() => 
        CurrentState.RightButton == ButtonState.Released && PreviousState.RightButton == ButtonState.Pressed;
    public bool MiddleReleased() => 
        CurrentState.MiddleButton == ButtonState.Released && PreviousState.MiddleButton == ButtonState.Pressed;
}