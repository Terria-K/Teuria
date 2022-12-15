using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class Sprite : Component
{
    private SpriteEffects spriteEffects = SpriteEffects.None;
    public Vector2 Scale = Vector2.One;
    public SpriteTexture Texture;
    public int Width { get; private set; }
    public int Height { get; private set; }
    public bool cleanUpTexture = false;
    public Color Modulate { get => Entity.Modulate; set => Entity.Modulate = value;  }
    public SpriteEffects SpriteEffects => spriteEffects;
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
    private bool useNinePatch;

    public Sprite(SpriteTexture texture, bool cleanUp = false, bool useNinePatch = false)
    {
        this.Texture = texture;
        Width = texture.Width;
        Height = texture.Height;
        cleanUpTexture = cleanUp;
        this.useNinePatch = useNinePatch;
    }

    public Sprite(SpriteTexture texture, int width, int height, bool cleanUp = false, bool useNinePatch = false)
    {
        this.Texture = texture;
        this.Width = width;
        this.Height = height;
        cleanUpTexture = cleanUp;
        this.useNinePatch = useNinePatch;
    }

    public override void Update() {}
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        if (useNinePatch)  
        {
            Texture.DrawTexture(spriteBatch, new Rectangle(
                (int)Entity.Position.X, 
                (int)Entity.Position.Y,
                Width, Height)
            );
            return;
        }
        Texture.DrawTexture(
            spriteBatch, 
            Entity.Position, 
            Modulate, 
            Rotation, 
            -PivotOffset, 
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

