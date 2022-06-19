using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Teuria;

public class Sprite : Component
{
    private SpriteEffects spriteEffects = SpriteEffects.None;
    public Vector2 Scale = Vector2.One;
    public Texture2D texture;
    private int width;
    private int height;
    public bool cleanUpTexture = false;
    public Color Modulate { get => Entity.Modulate; set => Entity.Modulate = value;  }
    public bool FlipH
    {
        get => (spriteEffects & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally;
        
        set => spriteEffects = value 
                ? spriteEffects | SpriteEffects.FlipHorizontally 
                : spriteEffects & ~SpriteEffects.FlipHorizontally;
        
    }
    public bool FlipV
    {
        get => (spriteEffects & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically;
        set => spriteEffects = value 
                ? spriteEffects | SpriteEffects.FlipVertically 
                : spriteEffects & ~SpriteEffects.FlipVertically;
    }


    public Sprite(Texture2D texture, bool cleanUp = false)
    {
        this.texture = texture;
        width = texture.Width;
        height = texture.Height;
        cleanUpTexture = cleanUp;
    }

    public Sprite(Texture2D texture, int width, int height, bool cleanUp = false)
    {
        this.texture = texture;
        this.width = width;
        this.height = height;
        cleanUpTexture = cleanUp;
    }

    public override void Update() {}
    public override void Draw(SpriteBatch spriteBatch)
    {
        
        spriteBatch.Draw(texture, Entity.Position, new Rectangle(0, 0, width, height), Modulate, 0, Vector2.Zero, Scale, spriteEffects, Entity.ZIndex);
    }

    public override void Removed()
    {
        if (cleanUpTexture) 
        {
            TextureImporter.CleanUp(texture);
        }
        base.Removed();
    }
}

