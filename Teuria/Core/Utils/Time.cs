using Microsoft.Xna.Framework;

namespace Teuria;

public static class Time 
{
    public static float DeltaScale = 1.0f;
    public static float Delta { get; internal set; }
    public static float FPS { get; internal set; }
    public static GameTime? DrawGameTime { get; internal set; }
    public static GameTime? UpdateGameTime { get; internal set; }
}