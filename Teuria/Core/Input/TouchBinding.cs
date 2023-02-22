#if ANDROID
using Microsoft.Xna.Framework;

namespace Teuria;

public class TouchBinding : IBinding
{
    internal Rectangle Rect;
    public bool JustPressed()
    {
        return TInput.Touch.JustPressed(Rect);
    }

    public bool Pressed()
    {
        return TInput.Touch.Pressed(Rect);
    }

    public bool Released()
    {
        return TInput.Touch.Released(Rect);
    }
}
#endif