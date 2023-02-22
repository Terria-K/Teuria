using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Teuria;

public class GamePadBinding : IBinding
{
    public List<Buttons> Controller = new List<Buttons>();
    public int gamepadIndex;

    public GamePadBinding(int gamepadIndex) { this.gamepadIndex = gamepadIndex; }
    public GamePadBinding(int gamepadIndex, params Buttons[] buttons) : this(gamepadIndex)
    {
        Add(buttons);
    }

    public void Add(params Buttons[] keys) 
    {
        foreach (var key in keys) 
        {
            if (Controller.Contains(key))
                continue;
            Controller.Add(key);
        }
    }

    public void Replace(params Buttons[] keys) 
    {
        if (Controller.Count > 0)
            Controller.Clear();

        Add(keys);
    }


    public bool JustPressed()
    {
        for (int i = 0; i < Controller.Count; i++) 
        {
            SkyLog.Assert(TInput.GamePads[gamepadIndex].DeviceConnected, "Device is not connected");
            if (TInput.GamePads[gamepadIndex].JustPressed(Controller[i]))
                return true;
        }


        return false;
    }

    public bool Pressed()
    {
        for (int i = 0; i < Controller.Count; i++)
            if (TInput.GamePads[gamepadIndex].Pressed(Controller[i]))
                return true;
        return false;
    }

    public bool Released()
    {
        for (int i = 0; i < Controller.Count; i++)
            if (TInput.GamePads[gamepadIndex].Released(Controller[i]))
                return true;
        return false;
    }
}

public class GamePadRightStickXAxisBinding : IAxisBinding
{
    public int GamepadIndex;
    public float Deadzone;

    public GamePadRightStickXAxisBinding(int gamepadIndex, float deadZone) 
    {
        GamepadIndex = gamepadIndex;
        Deadzone = deadZone;
    }

    public int GetValue()
    {
        var hori = TInput.GamePads[GamepadIndex].GetRightStick().X;
        if (Math.Abs(hori) >= Deadzone)
            return Math.Sign(hori);
        return 0;
    }

    public void Update() {}
}

public class GamePadLeftStickXAxisBinding : IAxisBinding
{
    public int GamepadIndex;
    public float Deadzone;

    public GamePadLeftStickXAxisBinding(int gamepadIndex, float deadZone) 
    {
        GamepadIndex = gamepadIndex;
        Deadzone = deadZone;
    }

    public int GetValue()
    {
        var hori = TInput.GamePads[GamepadIndex].GetLeftStick().X;
        if (Math.Abs(hori) >= Deadzone)
            return Math.Sign(hori);
        return 0;
    }

    public void Update() {}
}

public class GamePadRightStickYAxisBinding : IAxisBinding
{
    public int GamepadIndex;
    public float Deadzone;

    public GamePadRightStickYAxisBinding(int gamepadIndex, float deadZone) 
    {
        GamepadIndex = gamepadIndex;
        Deadzone = deadZone;
    }

    public int GetValue()
    {
        var hori = TInput.GamePads[GamepadIndex].GetRightStick().Y;
        if (Math.Abs(hori) >= Deadzone)
            return Math.Sign(hori);
        return 0;
    }

    public void Update() {}
}

public class GamePadLeftStickYAxisBinding : IAxisBinding
{
    public int GamepadIndex;
    public float Deadzone;

    public GamePadLeftStickYAxisBinding(int gamepadIndex, float deadZone) 
    {
        GamepadIndex = gamepadIndex;
        Deadzone = deadZone;
    }

    public int GetValue()
    {
        var hori = TInput.GamePads[GamepadIndex].GetLeftStick().Y;
        if (Math.Abs(hori) >= Deadzone)
            return Math.Sign(hori);
        return 0;
    }

    public void Update() {}
}