using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using static Teuria.AxisButton;

namespace Teuria;

public class KeyAxisBinding : IAxisBinding
{
    public Keys Negative;
    public Keys Positive;
    public WhenOverlap WhenOverlap;
    private int value;
    private bool isTurned;

    public KeyAxisBinding(Keys negative, Keys positive, WhenOverlap overlap) 
    {
        Negative = negative;
        Positive = positive;
        WhenOverlap = overlap;
    }

    public int GetValue()
    {
        return value;
    }

    public void Update() {
        var negative = TInput.Keyboard.Pressed(Negative);
        var positive = TInput.Keyboard.Pressed(Positive);

        if (negative && positive) 
        {
            switch (WhenOverlap) 
            {
                case WhenOverlap.Cancel:
                    value = 0;
                    return;
                case WhenOverlap.Newer when !isTurned:
                    value *= -1;
                    isTurned = true;
                    return;
                case WhenOverlap.Older:
                    return;
            }
        }
        if (positive) 
        {
            isTurned = false;
            value = 1;
            return;
        }
        if (negative) 
        {
            isTurned = false;
            value = -1;
            return;
        }
        isTurned = false;
        value = 0;
    }
}

public class KeyBinding : IBinding
{
    public List<Keys> Keys = new List<Keys>();

    public KeyBinding() {}
    public KeyBinding(params Keys[] keys) 
    {
        Add(keys);
    }

    public void Add(params Keys[] keys) 
    {
        foreach (var key in keys) 
        {
            if (Keys.Contains(key))
                continue;
            Keys.Add(key);
        }
    }

    public void Replace(params Keys[] keys) 
    {
        if (Keys.Count > 0)
            Keys.Clear();

        Add(keys);
    }

    public bool Pressed() 
    {
        foreach (var key in Keys) 
        {
            if (TInput.Keyboard.Pressed(key))
                return true;
        }
        return false;
    }

    public bool JustPressed() 
    {
        foreach (var key in Keys) 
        {
            if (TInput.Keyboard.JustPressed(key))
                return true;
        }
        return false;
    }

    public bool Released() 
    {
        foreach (var key in Keys) 
        {
            if (TInput.Keyboard.Released(key))
                return true;
        }
        return false;
    }
}