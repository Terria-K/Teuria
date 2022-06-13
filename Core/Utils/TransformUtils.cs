using Microsoft.Xna.Framework;

namespace Teuria;

public static class TransformUtils 
{
    public static Vector2 Center(float offsetX = 0, float offsetY = 0) 
    {
        return new Vector2((TeuriaEngine.screenWidth / 2) - offsetX, (TeuriaEngine.screenHeight / 2) - offsetY);
    }
}