using Microsoft.Xna.Framework.Input;

namespace Teuria;

public class KeyboardInput : BaseInput
{
    public KeyboardState PreviousState;
    public KeyboardState CurrentState;

    public override void Update()
    {
        PreviousState = CurrentState;
        CurrentState = Keyboard.GetState();
    }

    public bool Pressed(Keys key) 
    {
        return CurrentState.IsKeyDown(key);
    }

    public bool JustPressed(Keys key) 
    {
        return CurrentState.IsKeyDown(key) && !PreviousState.IsKeyDown(key);
    }

    public bool Released(Keys key) 
    {
        return !CurrentState.IsKeyDown(key) && PreviousState.IsKeyDown(key);
    }

    public int GetAxis(Keys neg, Keys pos) 
    {
        return Pressed(neg) 
            ? (Pressed(pos) ? 0 : -1) 
            : (Pressed(pos) ? 1 : 0);
    }
}