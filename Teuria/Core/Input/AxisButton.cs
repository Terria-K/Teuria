namespace Teuria;

public class AxisButton : BindableInput 
{
    public WeakList<IAxisBinding> AxisBindings = new WeakList<IAxisBinding>();
    public int Value;
    public int PreviousValue;

    public AxisButton(IAxisBinding axisBinding) 
    {
        AxisBindings.Add(axisBinding);
    }

    public AxisButton(params IAxisBinding[] axisBindings) 
    {
        Add(axisBindings);
    }

    public void Add(params IAxisBinding[] binding) 
    {
        for (int i = 0; i < binding.Length; i++) 
        {
            var axis = binding[i];
            AxisBindings.Add(axis);
        }
    }

    public override void Delete()
    {
        AxisBindings.Clear();
    }

    public override void Update()
    {
        for (int i = 0; i < AxisBindings.Count; i++) 
        {
            AxisBindings[i].Update();
        }


        PreviousValue = Value;
        Value = 0;

        for (int i = 0; i < AxisBindings.Count; i++) 
        {
            var value = AxisBindings[i].GetValue();
            if (value != 0) 
            {
                Value = value;
                break;
            }
        }
    }
}