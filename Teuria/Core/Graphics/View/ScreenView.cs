using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

//TODO Implement this in GameApp
public abstract class ScreenView 
{
    public GraphicsDevice GraphicsDevice { get; init; }
    public int Width;
    public int Height;
    public int ViewWidth => GraphicsDevice.Viewport.Width;
    public int ViewHeight => GraphicsDevice.Viewport.Height;
    public Viewport Viewport => GraphicsDevice.Viewport;
    public Rectangle BoundingRect => new Rectangle(0, 0, Width, Height);
    public Point Center => BoundingRect.Center;
    public ScreenView(GraphicsDevice device) 
    {
        GraphicsDevice = device;
    }

    public abstract Matrix CalculateMatrix();

    public virtual Point PointToScreen(int x, int y) 
    {
        var matrix = CalculateMatrix();
        return Vector2.Transform(new Vector2(x, y), Matrix.Invert(matrix)).ToPoint();
    }

    public virtual void GraphicsReset() {}
}