using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Teuria;

public class Input
{
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

