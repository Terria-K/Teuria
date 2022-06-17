using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Teuria;

public class Input
{
    private Dictionary<string, Keys> inputMap = new Dictionary<string, Keys>();

    public Keys this[string key] { get => inputMap[key]; set => inputMap[key] = value; }

    public void AddInput(string keyName, Keys key) 
    {
        inputMap[keyName] = key;
    }

    public Keys Left { get; set; }
    public Keys Right { get; set; }
    public Keys Down { get; set; }
    public Keys Up { get; set; }



    public bool IsKeyDown(Keys keys) 
    {
        return Keyboard.GetState().IsKeyDown(keys);
    }

    public bool IsKeyUp(Keys keys) 
    {
        return Keyboard.GetState().IsKeyUp(keys);
    }
}

