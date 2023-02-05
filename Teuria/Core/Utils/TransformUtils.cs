using Microsoft.Xna.Framework;

namespace Teuria;

public static class TransformUtils 
{
    public static Vector2 Center(float offsetX = 0, float offsetY = 0) 
    {
        return new Vector2((GameApp.ScreenWidth / 2) - offsetX, (GameApp.ScreenHeight / 2) - offsetY);
    }
}