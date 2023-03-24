using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Teuria;
public enum KeyboardOverlap { Cancel, Newer, Older }

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