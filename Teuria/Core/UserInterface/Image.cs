using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public abstract class BaseImage : Control 
{
    public required SpriteTexture Texture;
}

public class AnimatedImage : BaseImage
{
    public required string AnimationPath;
    private AnimatedSprite? animSprite;

    public override void EnterScene(Scene scene, ContentManager content)
    {
        base.EnterScene(scene, content);
        if (RectSize != Vector2.Zero) 
        {
            animSprite = new AnimatedSprite(AnimationPath, Texture);
        }
    }
}

public class UIImage : BaseImage
{
    private Sprite? sprite;


    public override void EnterScene(Scene scene, ContentManager content)
    {
        base.EnterScene(scene, content);
        if (RectSize != Vector2.Zero)
            sprite = new Sprite(Texture, (int)RectSize.X, (int)RectSize.Y, false, true);
        else 
        {
            sprite = new Sprite(Texture, false, false);
            RectSize = new Vector2(sprite.Width, sprite.Height);
        }

        AddComponent(sprite);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
    }
}