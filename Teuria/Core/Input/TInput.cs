using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;

namespace Teuria;

public static class TInput 
{
    private static Dictionary<string, Keys> keyActions = new Dictionary<string, Keys>();
    internal static List<BaseInput> InputList = new List<BaseInput>();
    internal static List<BindableInput> BindableInputs = new List<BindableInput>();
    public static KeyboardInput Keyboard { get; private set; }
    public static MouseInput Mouse { get; private set; }
#if ANDROID
    public static TouchInput Touch { get; private set; }
#endif

    public static bool Disabled = false;

    internal static void Initialize() 
    {
        Keyboard = new KeyboardInput();
        Mouse = new MouseInput();
        AddInput(Keyboard);
        AddInput(Mouse);
#if ANDROID
        Touch = new TouchInput();
        AddInput(Touch);
#endif
    }

    internal static void Update() 
    {
        foreach (var input in InputList) 
        {
            input.Update();
        }

        foreach (var bindableInput in BindableInputs) 
        {
            bindableInput.Update();
        }
    }

    public static void AddInput(BaseInput baseInput) 
    {
        InputList.Add(baseInput);
    }

    public static void AppendAction(string name, Keys keys) 
    {
        Debug.Assert(keyActions.ContainsKey(name) && keyActions.ContainsValue(keys), 
        $"The action keys named {name} has already exists with a same key as {keys}.");
        keyActions[name] = keys;
    }

#region Action Helper
    public static bool IsActionPressed(string name) 
    {
        if (!keyActions.ContainsKey(name)) { return false; }
        return Keyboard.Pressed(keyActions[name]);
    }

    public static bool IsActionReleased(string name) 
    {
        if (!keyActions.ContainsKey(name)) { return false; }
        return Keyboard.Released(keyActions[name]);
    }

    public static bool IsActionJustPressed(string name) 
    {
        if (!keyActions.ContainsKey(name)) { return false; }
        return Keyboard.JustPressed(keyActions[name]);
    }

#endregion

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