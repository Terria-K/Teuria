using Microsoft.Xna.Framework;

namespace Teuria;

public static class TransformUtils 
{
    public static Vector2 Center(float offsetX = 0, float offsetY = 0) 
    {
        return new Vector2((TeuriaEngine.ScreenWidth / 2) - offsetX, (TeuriaEngine.ScreenHeight / 2) - offsetY);
    }
}