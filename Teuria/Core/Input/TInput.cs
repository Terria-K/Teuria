using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Teuria;

public static class TInput 
{
    internal static List<BaseInput> InputList = new List<BaseInput>();
    internal static List<BindableInput> BindableInputs = new List<BindableInput>();
    public static KeyboardInput Keyboard { get; private set; }
    public static MouseInput Mouse { get; private set; }
    public static GamePadInput[] GamePads { get; private set; }
    public static TouchInput Touch { get; private set; } = null!;

    public static bool Disabled = false;

    static TInput() 
    {
        Keyboard = new KeyboardInput();
        Mouse = new MouseInput();
        GamePads = new GamePadInput[4];
#if ANDROID || IOS
        Touch = new TouchInput();
#endif
#if DEBUG
        TouchPanel.EnableMouseGestures = true;
        TouchPanel.EnableMouseTouchPoint = true;
#endif
    }

    internal static void Initialize() 
    {

        AddInput(Keyboard);
        AddInput(Mouse);
        for (int i = 0; i < 4; i++) 
        {
            GamePads[i] = new GamePadInput((PlayerIndex)i);
            AddInput(GamePads[i]);
        }

#if ANDROID || IOS
        AddInput(Touch);
#endif
    }

    internal static void Shutdown() 
    {
        foreach (var gamepad in GamePads) 
        {
            gamepad.StopVibrate();
        }
    }

    internal static void Update() 
    {
        if (Disabled)
            return;
        foreach (var input in InputList) 
        {
            input.Update();
        }

        foreach (var bindableInput in BindableInputs) 
        {
            bindableInput.UpdateInternal();
        }
    }

    public static void AddInput(BaseInput baseInput) 
    {
        InputList.Add(baseInput);
    }


#region Keyboard Helper
    public static bool IsKeyPressed(Keys keys) 
    {
        return Keyboard.Pressed(keys);
    }

    public static bool IsKeyReleased(Keys keys) 
    {
        return Keyboard.Released(keys);
    }

    public static bool IsKeyJustPressed(Keys keys) 
    {
        return Keyboard.JustPressed(keys);
    }
#endregion
}