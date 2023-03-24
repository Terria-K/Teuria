using Microsoft.Xna.Framework.Input;

namespace Teuria;

public class KeyboardAxisBinding : IAxisBinding
{
    public Keys Negative;
    public Keys Positive;
    public KeyboardOverlap WhenOverlap;
    private int value;
    private bool isTurned;
    private int button;
    private bool intercepted;

    public KeyboardAxisBinding(Keys negative, Keys positive, KeyboardOverlap overlap) 
    {
        Negative = negative;
        Positive = positive;
        WhenOverlap = overlap;
    }

    public KeyboardAxisBinding(int negative, int positive, KeyboardOverlap overlap) 
    {
        Negative = (Keys)negative;
        Positive = (Keys)positive;
        WhenOverlap = overlap;
    }

    public int GetValue()
    {
        return value;
    }

    public void Update() {
        bool negative;
        bool positive;
        if (!intercepted) 
        {
            negative = TInput.Keyboard.Pressed(Negative);
            positive = TInput.Keyboard.Pressed(Positive);
        }
        else 
        {
            negative = button == 0; 
            positive = button == 1;
            intercepted = false;
        }

        if (negative && positive) 
        {
            switch (WhenOverlap) 
            {
                case KeyboardOverlap.Cancel:
                    value = 0;
                    return;
                case KeyboardOverlap.Newer when !isTurned:
                    value *= -1;
                    isTurned = true;
                    return;
                case KeyboardOverlap.Older:
                    return;
            }
        }
        if (positive) 
        {
            isTurned = false;
            value = 1;
            return;
        }
        if (negative) 
        {
            isTurned = false;
            value = -1;
            return;
        }
        isTurned = false;
        value = 0;
    }

    public void Intercept(int button) 
    {
        this.button = button;
        intercepted = true;
    }
}
