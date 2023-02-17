using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Teuria;

public class KeyBinding : Binding 
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

    public override bool Pressed() 
    {
        foreach (var key in Keys) 
        {
            if (TInput.Keyboard.Pressed(key))
                return true;
        }
        return false;
    }

    public override bool JustPressed() 
    {
        foreach (var key in Keys) 
        {
            if (TInput.Keyboard.JustPressed(key))
                return true;
        }
        return false;
    }

    public override bool Released() 
    {
        foreach (var key in Keys) 
        {
            if (TInput.Keyboard.Released(key))
                return true;
        }
        return false;
    }
}