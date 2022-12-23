namespace Teuria;

public class AxisButton : BindableInput 
{
    public Binding Negative;
    public Binding Positive;
    public int Value;
    public int PreviousValue;
    private bool isTurned;
    private WhenOverlap whenOverlap;

    public AxisButton(Binding negative, Binding positive, WhenOverlap overlap)
    {
        Negative = negative;
        Positive = positive;
        whenOverlap = overlap;
    }

    public override void Update()
    {
        PreviousValue = Value;
        if (TInput.Disabled) return;


        var negative = Negative.Pressed();
        var positive = Positive.Pressed();

        if (negative && positive) 
        {
            switch (whenOverlap) 
            {
                case WhenOverlap.Cancel:
                    Value = 0;
                    break;
                case WhenOverlap.Newer when !isTurned:
                    Value *= -1;
                    isTurned = true;
                    break;
                case WhenOverlap.Older:
                    Value = PreviousValue;
                    break;
            }
        }
        else if (positive) 
        {
            isTurned = false;
            Value = 1;
        }
        else if (negative) 
        {
            isTurned = false;
            Value = -1;
        }
        else 
        {
            isTurned = false;
            Value = 0;
        }
    }

    public enum WhenOverlap { Cancel, Newer, Older }
}