using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Teuria;
public enum KeyboardOverlap { Cancel, Newer, Older }

public class KeyboardAxisBinding : IAxisBinding
{
    public Keys Negative;
    public Keys Positive;
    public KeyboardOverlap WhenOverlap;
    private int value;
    private bool isTurned;

    public KeyboardAxisBinding(Keys negative, Keys positive, KeyboardOverlap overlap) 
    {
        Negative = negative;
        Positive = positive;
        WhenOverlap = overlap;
    }

    public KeyboardAxisBinding(int negative, int positive, KeyboardOverlap overlap) 
    {
        Negative = (Keys)negative;
        Positive = (Keys)positive;
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
                case KeyboardOverlap.Cancel:
                    value = 0;
                    return;
                case KeyboardOverlap.Newer when !isTurned:
                    value *= -1;
                    isTurned = true;
                    return;
                case KeyboardOverlap.Older:
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

public class KeyboardBinding : IBinding
{
    public List<Keys> Keys = new List<Keys>();

    public KeyboardBinding() {}
    public KeyboardBinding(params Keys[] keys) 
    {
        Add(keys);
    }

    public KeyboardBinding(params int[] keys) 
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

    public void Add(params int[] keys) 
    {
        foreach (var key in keys) 
        {
            if (Keys.Contains((Keys)key))
                continue;
            Keys.Add((Keys)key);
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