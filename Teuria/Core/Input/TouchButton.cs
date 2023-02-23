using Microsoft.Xna.Framework;

namespace Teuria;

public class TouchButton : BindableInput
{
    public TouchBinding Binding;
    public float BufferTime;
    private float buffer;
    private Rectangle touchArea;
    private bool active;

    public TouchButton(TouchBinding binding, float bufferTime) 
    {
        Binding = binding;
        BufferTime = bufferTime;
    }

    public void ActivateButton(Rectangle rect) 
    {
        active = true;
        touchArea = rect;
        Binding.Rect = rect;
    }

    public bool JustPressed => Binding.JustPressed() || buffer > 0f;
    public bool Pressed => Binding.Pressed();
    public bool Released => Binding.Released();
    public override void Update()
    {
        if (!active)
            return;
        buffer -= Time.Delta;
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

    public override void Delete()
    {
    }
}