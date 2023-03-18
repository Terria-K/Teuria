using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Teuria;

public class MouseInput : BaseInput
{
    public MouseState PreviousState;
    public MouseState CurrentState;

    internal MouseInput() {}

    public override void Update()
    {
        PreviousState = CurrentState;
        CurrentState = Mouse.GetState();
    }

    public int Scroll => CurrentState.ScrollWheelValue - PreviousState.ScrollWheelValue;
    public int ScrollValue => CurrentState.ScrollWheelValue;

    [Obsolete("Use GetPosition(Camera camera) instead")]
    public Vector2 Position 
    {
        get => Vector2.Transform(new Vector2(CurrentState.X, CurrentState.Y), Matrix.Invert(GameApp.ScreenMatrix));
        set 
        {
            var vector = Vector2.Transform(value, GameApp.ScreenMatrix);
            Mouse.SetPosition((int)Math.Round(vector.X), (int)Math.Round(vector.Y));  
        } 
    }

    public Vector2 GetViewportMousePosition() 
    {
        var mouseState = Mouse.GetState();
        float currentX = mouseState.X - GameApp.Instance.Screen.X;
        float currentY = mouseState.Y - GameApp.Instance.Screen.Y;

        var mousePosX = currentX / GameApp.Instance.Screen.Width * GameApp.ViewWidth;
        var mousePosY = currentY / GameApp.Instance.Screen.Height * GameApp.ViewHeight;

        return new Vector2(mousePosX, mousePosY);
    }

    public Vector2 GetCanvasMousePosition(Camera camera) => GetPosition(GetViewportMousePosition(), camera);


    public Vector2 GetPosition(Vector2 position, Camera camera) 
    {
        return Vector2.Transform(position, Matrix.Invert(camera.Transform));
    }


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