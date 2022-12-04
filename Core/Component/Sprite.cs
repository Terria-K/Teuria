using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class Sprite : Component
{
    private SpriteEffects spriteEffects = SpriteEffects.None;
    public Vector2 Scale = Vector2.One;
    public SpriteTexture Texture;
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

    public Vector2 PivotOffset { get; set; }

    public float Rotation { get; set; }

    public Sprite(SpriteTexture texture, bool cleanUp = false)
    {
        this.Texture = texture;
        width = texture.Width;
        height = texture.Height;
        cleanUpTexture = cleanUp;
    }

    public Sprite(SpriteTexture texture, int width, int height, bool cleanUp = false)
    {
        this.Texture = texture;
        this.width = width;
        this.height = height;
        cleanUpTexture = cleanUp;
    }

    public override void Update() {}
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        Texture.DrawTexture(
            spriteBatch, 
            Entity.Position + PivotOffset, 
            Modulate, 
            Rotation, 
            PivotOffset, 
            Scale, 
            spriteEffects, 
            Entity.ZIndex
        );
    }

    public override void Removed()
    {
        if (cleanUpTexture) 
        {
            TextureImporter.CleanUp(Texture.Texture);
        }
        base.Removed();
    }
}

