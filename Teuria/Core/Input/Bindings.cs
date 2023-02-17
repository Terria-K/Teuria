using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Teuria;

public abstract class Binding
{
    // public List<Keys> Keys = new List<Keys>();

    // public Binding() {}
    // public Binding(params Keys[] keys) 
    // {
    //     Add(keys);
    // }

    // public void Add(params Keys[] keys) 
    // {
    //     foreach (var key in keys) 
    //     {
    //         if (Keys.Contains(key))
    //             continue;
    //         Keys.Add(key);
    //     }
    // }

    // public void Replace(params Keys[] keys) 
    // {
    //     if (Keys.Count > 0)
    //         Keys.Clear();

    //     Add(keys);
    // }

    public abstract bool Pressed();
    

    public abstract bool JustPressed(); 

    public abstract bool Released();
}