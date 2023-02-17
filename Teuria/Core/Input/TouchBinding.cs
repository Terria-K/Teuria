#if ANDROID
using Microsoft.Xna.Framework;

namespace Teuria;

public class TouchBinding : Binding
{
    internal Rectangle Rect;
    public override bool JustPressed()
    {
        return TInput.Touch.JustPressed(Rect);
    }

    public override bool Pressed()
    {
        return TInput.Touch.Pressed(Rect);
    }

    public override bool Released()
    {
        return TInput.Touch.Released(Rect);
    }
}
#endif