using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class BoxingScreenView : ScreenView
{
    private readonly GameWindow window;
    public int HorizontalBleed;
    public int VerticalBleed;
    public BoxingScreenView(GraphicsDevice device, GameWindow window, int width, int height) : base(device) 
    {
        this.window = window;
        Width = width;
        Height = height;
    }

    public override Matrix CalculateMatrix()
    {
        return Matrix.CreateScale(ViewWidth/Width, ViewHeight/Height, 1.0f);
    }

    public override void GraphicsReset()
    {
        base.GraphicsReset();
        var clientBounds = window.ClientBounds;

        var worldScaleX = (float)clientBounds.Width / Width;
        var worldScaleY = (float)clientBounds.Height / Height;

        var safeScaleX = (float)clientBounds.Width / (Width - HorizontalBleed);
        var safeScaleY = (float)clientBounds.Height / (Height - VerticalBleed);

        var worldScale = MathHelper.Max(worldScaleX, worldScaleY);
        var safeScale = MathHelper.Min(safeScaleX, safeScaleY);
        var scale = MathHelper.Min(worldScale, safeScale);

        var width = (int)(scale * Width + 0.5f);
        var height = (int)(scale * Height + 0.5f);

        var x = clientBounds.Width / 2 - width / 2;
        var y = clientBounds.Height / 2 - height / 2;
        GraphicsDevice.Viewport = new Viewport(x, y, width, height);
    }

    public override Point PointToScreen(int x, int y)
    {
        return base.PointToScreen(x - GraphicsDevice.Viewport.X, y - GraphicsDevice.Viewport.Y);
    }
}