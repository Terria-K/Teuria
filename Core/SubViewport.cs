using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class SubViewport 
{
    public Point ViewResolution 
    { 
        get => viewResolution; 
        set 
        {
            viewResolution = value;
            needUpdate = true;
        } 
    }
    private Point viewResolution;
    public Point ScreenResolution 
    {
        get => screenResolution;
        set 
        {
            screenResolution = value;
            needUpdate = true;
        }
    }
    private Point screenResolution;
    private bool needUpdate;

    private Color environmentColor;
    private Vector2 scale;
    private GraphicsDevice device;
    private RenderTarget2D rt;
    private SpriteBatch spriteBatch;
    private RenderTargetBinding[] prevTargets;

    public SubViewport(Point viewResolution, GraphicsDevice device, Color environmentColor) 
    {
        this.device = device;
        this.viewResolution = viewResolution;
        screenResolution = new Point(device.Viewport.Width, device.Viewport.Height);
        this.environmentColor = environmentColor;

        spriteBatch = new SpriteBatch(device);
        needUpdate = false;
    }

    private void UpdateResolution() 
    {
        needUpdate = false;
        scale = ScreenResolution.ToVector2() / ViewResolution.ToVector2();
        rt = new RenderTarget2D(device, viewResolution.X, viewResolution.Y);
    }

    internal void UpdateResolution(ref Viewport viewport) 
    {
        screenResolution.X = viewport.Width;
        screenResolution.Y = viewport.Height;
        needUpdate = true;
    }

    public void Begin() 
    {
        if (needUpdate) 
        {
            UpdateResolution();
        }

        prevTargets = device.GetRenderTargets();
        device.SetRenderTarget(rt);
        device.Clear(environmentColor);
    }

    public void End() 
    {
        var pos = ScreenResolution.ToVector2() / 2f;
        var origin = ViewResolution.ToVector2() / 2f;

        device.SetRenderTargets(prevTargets);
        device.Clear(environmentColor);
        spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend);
        spriteBatch.Draw(rt, pos, null, Color.White, 0, origin, scale, SpriteEffects.None, 0);
        spriteBatch.End();
    }
}