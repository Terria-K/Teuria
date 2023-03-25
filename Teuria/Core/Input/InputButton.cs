namespace Teuria;

public abstract class BindableInput
{
    protected bool InterceptedOnPress;
    public BindableInput() 
    {
        TInput.BindableInputs.Add(this);
    }
    internal void UpdateInternal() 
    {
        Update();
    }
    public abstract void Update();
    public abstract void Delete();

    public void InterceptPress() 
    {
        InterceptedOnPress = true;
    }
}

public class InputButton : BindableInput 
{
    public WeakList<IBinding> Bindings = new WeakList<IBinding>();
    public float BufferTime;
    private float buffer;
    private bool intercepted;

    public bool JustPressed 
    {
        get 
        {
            if (TInput.Disabled)
                return false;
            if (InterceptedOnPress || buffer > 0f) 
            {
                intercepted = true;
                return true;
            }


            for (int i = 0; i < Bindings.Count; i++) 
            {
                if (Bindings[i].JustPressed() || buffer > 0f)
                    return true;
            }
            return false;
        }
    }

    public bool Pressed 
    {
        get 
        {
            if (TInput.Disabled)
                return false;

            if (InterceptedOnPress) 
            {
                intercepted = true;
                return true;
            }


            for (int i = 0; i < Bindings.Count; i++) 
            {
                if (Bindings[i].Pressed())
                    return true;
            }
            return false;
        }
    }

    public bool Released 
    {
        get 
        {
            if (TInput.Disabled)
                return false;

            for (int i = 0; i < Bindings.Count; i++) 
            {
                if (Bindings[i].Released())
                    return true;
            }
            return false;
        }
    }

    public InputButton(IBinding binding, float bufferTime) 
    {
        this.Bindings.Add(binding);
        BufferTime = bufferTime;
    }

    public InputButton(IBinding[] binding, float bufferTime) 
    {
        foreach (var bind in binding)
            this.Bindings.Add(bind);
        BufferTime = bufferTime;
    }

    public override void Update() 
    {
        buffer -= Time.Delta;
        var pressed = false;
        for (int i = 0; i < Bindings.Count; i++) 
        {
            var binding = Bindings[i];
            if (binding.Pressed()) 
            {
                buffer = BufferTime;
            }
            else if (binding.JustPressed()) 
            {
                buffer = BufferTime;
                pressed = true;
            }
        }
        if (!pressed) 
            buffer = 0;
        if (intercepted) 
        {
            InterceptedOnPress = false;
            intercepted = false;
        }

    }

    public override void Delete()
    {
        Bindings.Clear();
    }
}