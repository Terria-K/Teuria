using Microsoft.Xna.Framework;

namespace Teuria;

public class TouchButton : InputButton
{
    public TouchBinding Binding;
    private float buffer;
    private Rectangle touchArea;
    private bool active;


    public TouchButton(TouchBinding binding, float bufferTime) : base(binding, bufferTime)
    {
        this.Binding = binding;
    }

    public void ActivateButton(Rectangle rect) 
    {
        active = true;
        touchArea = rect;
        Binding.Rect = rect;
    }

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