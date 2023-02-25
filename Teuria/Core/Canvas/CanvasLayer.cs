using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public abstract class CanvasLayer 
{
    public Scene? Scene;
    public bool Visible = true;

    public virtual void PreDraw(Scene scene, SpriteBatch spriteBatch) {}
    public abstract void Draw(Scene scene, SpriteBatch spriteBatch);
    public virtual void PostDraw(Scene scene) {}
    public virtual void Unload() {}

    internal void Obtain(Scene scene) 
    {
        Scene = scene;
        Ready();
    }

    public virtual void Ready() 
    {

    }
}