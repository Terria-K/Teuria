namespace Teuria;

public abstract class BindableInput
{
    public BindableInput() 
    {
        TInput.BindableInputs.Add(this);
    }
    public abstract void Update();
}

public class InputButton : BindableInput 
{
    public Binding Binding;
    public float BufferTime;
    private float buffer;

    public bool JustPressed => Binding.JustPressed() || buffer > 0f;
    public bool Pressed => Binding.Pressed();
    public bool Released => Binding.Released();

    public InputButton(Binding binding, float bufferTime) 
    {
        this.Binding = binding;
        BufferTime = bufferTime;
    }

    public override void Update() 
    {
        buffer -= TeuriaEngine.DeltaTime;
        var pressed = false;
        if (Binding.Pressed()) 
        {
            buffer = BufferTime;
        }
        else if (Binding.JustPressed()) 
        {
            buffer = BufferTime;
            pressed = true;
        }
        if (!pressed) 
            buffer = 0;
        
    }
}