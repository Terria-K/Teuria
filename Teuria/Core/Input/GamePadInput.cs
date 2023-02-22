using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Teuria;

public class GamePadInput : BaseInput
{
    public readonly PlayerIndex Index;
    public GamePadState PreviousState;
    public GamePadState CurrentState;
    public bool DeviceConnected;


    private float vibrationStrength;
    private float vibrationTime;

    internal GamePadInput(PlayerIndex index) 
    {
        Index = index;
    }


    public override void Update()
    {
        PreviousState = CurrentState;
        CurrentState = GamePad.GetState(Index);
        DeviceConnected = CurrentState.IsConnected;

        if (vibrationTime <= 0)
            return;
        
        vibrationTime -= Time.Delta;
        if (vibrationTime <= 0)
            GamePad.SetVibration(Index, 0, 0);
    }

    public bool Pressed(Buttons button) 
    {
        if (Disabled)
            return false;
        return CurrentState.IsButtonDown(button);
    } 

    public bool JustPressed(Buttons button) 
    {
        if (Disabled)
            return false;
        return CurrentState.IsButtonDown(button) && PreviousState.IsButtonUp(button);
    }

    public bool Released(Buttons button) 
    {
        if (Disabled)
            return false;
        return CurrentState.IsButtonUp(button) && PreviousState.IsButtonDown(button);
    }

    public Vector2 GetLeftStick() 
    {
        var thmb = CurrentState.ThumbSticks.Left;
        thmb.Y = -thmb.Y;
        return thmb;
    }

    public Vector2 GetLeftStick(float deadZone) 
    {
        var thmb = CurrentState.ThumbSticks.Left;
        if (thmb.LengthSquared() < deadZone * deadZone)
            return Vector2.Zero;

        thmb.Y = -thmb.Y;
        return thmb;
    }

    public Vector2 GetRightStick() 
    {
        var thmb = CurrentState.ThumbSticks.Right;
        thmb.Y = -thmb.Y;
        return thmb;
    }

    public Vector2 GetRightStick(float deadZone) 
    {
        var thmb = CurrentState.ThumbSticks.Right;
        if (thmb.LengthSquared() < deadZone * deadZone)
            return Vector2.Zero;

        thmb.Y = -thmb.Y;
        return thmb;
    }
#region LeftStick

    public bool LeftStickLeftPressed(float deadZone) 
    {
        return CurrentState.ThumbSticks.Left.X <= -deadZone;
    }

    public bool LeftStickLeftJustPressed(float deadZone) 
    {
        return CurrentState.ThumbSticks.Left.X <= -deadZone && PreviousState.ThumbSticks.Left.X > -deadZone;
    }

    public bool LeftStickLeftReleased(float deadZone) 
    {
        return CurrentState.ThumbSticks.Left.X > -deadZone && PreviousState.ThumbSticks.Left.X <= -deadZone;
    }

    public bool LeftStickRightPressed(float deadZone) 
    {
        return CurrentState.ThumbSticks.Left.X >= -deadZone;
    }

    public bool LeftStickRightJustPressed(float deadZone) 
    {
        return CurrentState.ThumbSticks.Left.X >= -deadZone && PreviousState.ThumbSticks.Left.X < -deadZone;
    }

    public bool LeftStickRightReleased(float deadZone) 
    {
        return CurrentState.ThumbSticks.Left.X < -deadZone && PreviousState.ThumbSticks.Left.X >= -deadZone;
    }

    public bool LeftStickDownPressed(float deadZone) 
    {
        return CurrentState.ThumbSticks.Left.Y <= -deadZone;
    }

    public bool LeftStickDownJustPressed(float deadZone) 
    {
        return CurrentState.ThumbSticks.Left.Y <= -deadZone && PreviousState.ThumbSticks.Left.X > -deadZone;
    }

    public bool LeftStickDownReleased(float deadZone) 
    {
        return CurrentState.ThumbSticks.Left.Y > -deadZone && PreviousState.ThumbSticks.Left.X <= -deadZone;
    }

    public bool LeftStickUpPressed(float deadZone) 
    {
        return CurrentState.ThumbSticks.Left.Y >= -deadZone;
    }

    public bool LeftStickUpJustPressed(float deadZone) 
    {
        return CurrentState.ThumbSticks.Left.Y >= -deadZone && PreviousState.ThumbSticks.Left.Y < -deadZone;
    }

    public bool LeftStickUpReleased(float deadZone) 
    {
        return CurrentState.ThumbSticks.Left.Y < -deadZone && PreviousState.ThumbSticks.Left.Y >= -deadZone;
    }

    public float LeftStickHorizontal(float deadZone) 
    {
        float h = CurrentState.ThumbSticks.Left.X;
        if (Math.Abs(h) < deadZone)
            return 0;
        return -h;
    }

    public float LeftStickVertical(float deadZone) 
    {
        float v = CurrentState.ThumbSticks.Left.Y;
        if (Math.Abs(v) < deadZone)
            return 0;
        return -v;
    }
#endregion

#region RightStick 

    public bool RightStickLeftPressed(float deadZone) 
    {
        return CurrentState.ThumbSticks.Right.X <= -deadZone;
    }

    public bool RightStickLeftJustPressed(float deadZone) 
    {
        return CurrentState.ThumbSticks.Right.X <= -deadZone && PreviousState.ThumbSticks.Right.X > -deadZone;
    }

    public bool RightStickLeftReleased(float deadZone) 
    {
        return CurrentState.ThumbSticks.Right.X > -deadZone && PreviousState.ThumbSticks.Right.X <= -deadZone;
    }

    public bool RightStickRightPressed(float deadZone) 
    {
        return CurrentState.ThumbSticks.Right.X >= -deadZone;
    }

    public bool RightStickRightJustPressed(float deadZone) 
    {
        return CurrentState.ThumbSticks.Right.X >= -deadZone && PreviousState.ThumbSticks.Right.X < -deadZone;
    }

    public bool RightStickRightReleased(float deadZone) 
    {
        return CurrentState.ThumbSticks.Right.X < -deadZone && PreviousState.ThumbSticks.Right.X >= -deadZone;
    }

    public bool RightStickDownPressed(float deadZone) 
    {
        return CurrentState.ThumbSticks.Right.Y <= -deadZone;
    }

    public bool RightStickDownJustPressed(float deadZone) 
    {
        return CurrentState.ThumbSticks.Right.Y <= -deadZone && PreviousState.ThumbSticks.Right.X > -deadZone;
    }

    public bool RightStickDownReleased(float deadZone) 
    {
        return CurrentState.ThumbSticks.Right.Y > -deadZone && PreviousState.ThumbSticks.Right.X <= -deadZone;
    }

    public bool RightStickUpPressed(float deadZone) 
    {
        return CurrentState.ThumbSticks.Right.Y >= -deadZone;
    }

    public bool RightStickUpJustPressed(float deadZone) 
    {
        return CurrentState.ThumbSticks.Right.Y >= -deadZone && PreviousState.ThumbSticks.Right.Y < -deadZone;
    }

    public bool RightStickUpReleased(float deadZone) 
    {
        return CurrentState.ThumbSticks.Right.Y < -deadZone && PreviousState.ThumbSticks.Right.Y >= -deadZone;
    }

    public float RightStickHorizontal(float deadZone) 
    {
        float h = CurrentState.ThumbSticks.Right.X;
        if (Math.Abs(h) < deadZone)
            return 0;
        return -h;
    }

    public float RightStickVertical(float deadZone) 
    {
        float v = CurrentState.ThumbSticks.Right.Y;
        if (Math.Abs(v) < deadZone)
            return 0;
        return -v;
    }
#endregion

#region DPad
    public int DPadHorizontalAxis() 
    {
        if (CurrentState.DPad.Right == ButtonState.Pressed)
            return 1;
        if (CurrentState.DPad.Left == ButtonState.Pressed)
            return -1;
        return 0;
    }

    public int DPadVerticalAxis() 
    {
        if (CurrentState.DPad.Down == ButtonState.Pressed)
            return 1;
        if (CurrentState.DPad.Up == ButtonState.Pressed)
            return -1;
        return 0;
    }

    public Vector2 DPadAxis() => new (DPadHorizontalAxis(), DPadVerticalAxis());

    public bool DPadLeftPressed() => CurrentState.DPad.Left == ButtonState.Pressed;

    public bool DPadLeftJustPressed() => CurrentState.DPad.Left == ButtonState.Pressed 
        && PreviousState.DPad.Left == ButtonState.Released;

    public bool DPadLeftReleased() => CurrentState.DPad.Left == ButtonState.Released
        && PreviousState.DPad.Left == ButtonState.Pressed;

    public bool DPadRightPressed() => CurrentState.DPad.Right == ButtonState.Pressed;

    public bool DPadRightJustPressed() => CurrentState.DPad.Right == ButtonState.Pressed 
        && PreviousState.DPad.Right == ButtonState.Released;

    public bool DPadRightReleased() => CurrentState.DPad.Right == ButtonState.Released
        && PreviousState.DPad.Right == ButtonState.Pressed;

    public bool DPadUpPressed() => CurrentState.DPad.Up == ButtonState.Pressed;

    public bool DPadUpJustPressed() => CurrentState.DPad.Up == ButtonState.Pressed 
        && PreviousState.DPad.Up == ButtonState.Released;

    public bool DPadUpReleased() => CurrentState.DPad.Up == ButtonState.Released
        && PreviousState.DPad.Up == ButtonState.Pressed;

    public bool DPadDownPressed() => CurrentState.DPad.Down == ButtonState.Pressed;

    public bool DPadDownJustPressed() => CurrentState.DPad.Down == ButtonState.Pressed 
        && PreviousState.DPad.Down == ButtonState.Released;

    public bool DPadDownReleased() => CurrentState.DPad.Down == ButtonState.Released
        && PreviousState.DPad.Down == ButtonState.Pressed;
#endregion

#region Triggers
    public bool LeftTriggerPressed(float threshold) 
    {
        if (Disabled)
            return false;
        return CurrentState.Triggers.Left >= threshold;
    }

    public bool LeftTriggerJustPressed(float threshold) 
    {
        if (Disabled)
            return false;
        return CurrentState.Triggers.Left >= threshold && PreviousState.Triggers.Left < threshold;
    }

    public bool LeftTriggerReleased(float threshold) 
    {
        if (Disabled)
            return false;
        return CurrentState.Triggers.Left < threshold && PreviousState.Triggers.Left >= threshold;
    }

    public bool RightTriggerPressed(float threshold) 
    {
        if (Disabled)
            return false;
        return CurrentState.Triggers.Right >= threshold;
    }

    public bool RightTriggerJustPressed(float threshold) 
    {
        if (Disabled)
            return false;
        return CurrentState.Triggers.Right >= threshold && PreviousState.Triggers.Right < threshold;
    }

    public bool RightTriggerReleased(float threshold) 
    {
        if (Disabled)
            return false;
        return CurrentState.Triggers.Right < threshold && PreviousState.Triggers.Right >= threshold;
    }
#endregion

    public void Vibrate(float strength, float time) 
    {
        if (
            vibrationTime <= 0 || 
            strength > vibrationStrength || 
            (strength == vibrationStrength && time > vibrationTime)
        ) {
            GamePad.SetVibration(Index, strength, strength);
            vibrationStrength = strength;
            vibrationTime = time;
        }
    }

    public void StopVibrate() 
    {
        GamePad.SetVibration(Index, 0, 0);
        vibrationTime = 0;
    }
}
        