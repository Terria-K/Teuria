using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;


public class UIImage : Control 
{
    public required SpriteTexture Texture;
    private Sprite? sprite;

    public override void Ready()
    {
        if (RectSize != Vector2.Zero)
            sprite = new Sprite(Texture, (int)RectSize.X, (int)RectSize.Y, false, true);
        else 
        {
            sprite = new Sprite(Texture, false, false);
            RectSize = new Vector2(sprite.Width, sprite.Height);
        }

        AddComponent(sprite);
        base.Ready();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
    }
}