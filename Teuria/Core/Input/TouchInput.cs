using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace Teuria;

public sealed class TouchInput : BaseInput
{
    private bool isTouch;
    public TouchCollection CurrentState;
    public TouchCollection PreviousState;
    private List<Button> buttons;
    public Action<Button>? OnReleased;
    public Action<Button>? OnPressed;
    public Action<Button, bool>? OnToggle;

#if DEBUG
    private bool notified;
#endif
    

    internal TouchInput() 
    {
        buttons = new List<Button>();
    }

    public override void Update()
    {
        PreviousState = CurrentState;
        CurrentState = TouchPanel.GetState();
        SkyLog.Log(CurrentState.Count);

        foreach (var state in CurrentState) 
        {
            foreach (var button in buttons) 
            {
                var pos = GetViewportMousePosition(state);
                SkyLog.Log(pos);
                var camera = GameApp.Instance.Scene.Camera;
                if (camera is null)
                {
#if DEBUG
                    if (!notified) 
                    {
                        SkyLog.Log("Mouse Input were disabled because there is no Camera in the scene", SkyLog.LogLevel.Warning);
                        notified = true;
                    }
#endif
                    break;
                }
                if (state.State == TouchLocationState.Released &&
                    button.TouchID == state.Id &&
                    !button.Toggleable) 
                {
                    button.TouchID = -1;

                    if (OnReleased != null && button.Contains(pos, camera)) 
                    {
                        OnReleased(button);
                    }
                }
                if (button.Contains(pos, camera) && state.State == TouchLocationState.Pressed) 
                {
                    if (button.Toggleable) 
                    {
                        button.Toggled = !button.Toggled;
                        OnToggle?.Invoke(button, button.Toggled);
                    }
                    else 
                    {
                        button.TouchID = state.Id;
                        button.Touched = true;
                        OnPressed?.Invoke(button);
                    }
                }
                if (button.TouchID == state.Id && state.State == TouchLocationState.Moved)
                    button.Touched = button.Contains(pos, camera);
            }

        }
    }

    public void AddButtonBox(Rectangle hitbox, bool toggleable = false) 
    {
        var button = new Button 
        {
            Toggleable = toggleable,
            Rectangle = hitbox
        };
        buttons.Add(button);
    }

    public void AddButtonBox(Button button) 
    {
        buttons.Add(button);
    }

    [Obsolete]
    public bool Pressed(Rectangle rectangle) 
    {
        return !Disabled && isTouch;
    }

    [Obsolete]
    public bool JustPressed(Rectangle rectangle) 
    {
        return !Disabled && CheckTouch(rectangle, CurrentState) && !CheckTouch(rectangle, PreviousState);
    }

    [Obsolete]
    public bool Released(Rectangle rectangle) 
    {
        return !Disabled && !CheckTouch(rectangle, CurrentState) && CheckTouch(rectangle, PreviousState);
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

    private static Vector2 ScreenToWorld(Vector2 position, Camera camera) 
    {
        return Vector2.Transform(position, Matrix.Invert(camera.Transform));
    }

    public static Vector2 GetViewportMousePosition(TouchLocation loc) 
    {
        var touchPos = loc.Position;
        float currentX = touchPos.X - GameApp.Instance.Screen.X;
        float currentY = touchPos.Y - GameApp.Instance.Screen.Y;

        var newTouchPosX = currentX / GameApp.Instance.Screen.Width * GameApp.ViewWidth;
        var newTouchPosY = currentY / GameApp.Instance.Screen.Height * GameApp.ViewHeight;

        return new Vector2(newTouchPosX, newTouchPosY);
    }

    public sealed class Button 
    {
        public int TouchID = -1;
        public Rectangle Rectangle;
        public bool Touched;
        public bool Toggleable;
        public bool Toggled;

        public bool IsPressed => TouchID >= 0;
        public bool IsJustPressed => IsPressed && Touched;
        public bool IsReleased => TouchID == -1;

        public bool Contains(Vector2 position, Camera camera) 
        {
            return Rectangle.Contains(ScreenToWorld(position, camera));
        }
    }
}