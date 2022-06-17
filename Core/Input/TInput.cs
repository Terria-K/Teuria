using System.Collections.Generic;

namespace Teuria;

public static class TInput 
{
    internal static List<BaseInput> InputList = new List<BaseInput>();
    public static KeyboardInput Keyboard { get; private set; }
    public static MouseInput Mouse { get; private set; }

    public static bool Active = false;
    public static bool Disabled = false;

    internal static void Initialize() 
    {
        Keyboard = new KeyboardInput();
        Mouse = new MouseInput();
        AddInput(Keyboard);
        AddInput(Mouse);
    }

    internal static void Update() 
    {
        foreach (var input in InputList) 
        {
            input.Update();
        }
    }

    public static void AddInput(BaseInput baseInput) 
    {
        InputList.Add(baseInput);
    }
}