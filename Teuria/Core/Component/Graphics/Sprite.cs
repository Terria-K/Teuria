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
    public Transform Transform = new Transform();
    public Color Modulate 
    { 
        get 
        {
            if (Entity != null) 
                return Entity.Modulate;

            throw new EntityDoesNotExistException();
        }  
        set 
        {
            if (Entity != null) 
            {
                Entity.Modulate = value;  
                return;
            }

            throw new EntityDoesNotExistException();
        } 
    }
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

    public float Rotation { get => Transform.Rotation; set => Transform.Rotation = value; }
    public Rectangle Rect;
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

    public Sprite(SpriteTexture texture, Rectangle clipRect, bool cleanUp = false, bool useNinePatch = false) 
    {
        Texture = texture;
        Width = clipRect.Width;
        Height = clipRect.Height;
        Rect = clipRect;
    }

    public override void Added(Entity entity)
    {
        Transform.Parent = entity.Transform;
        base.Added(entity);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Entity == null)
            return;
        if (useNinePatch)  
        {
            Texture.DrawTexture(spriteBatch, new Rectangle(
                (int)(Transform.Position.X + PivotOffset.X), 
                (int)(Transform.Position.Y + PivotOffset.Y),
                Width, Height), 
                Modulate
            );
            return;
        }
        Texture.DrawTexture(
            spriteBatch, 
            Transform.Position,
            Rect,
            Modulate, 
            Transform.Rotation, 
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
            TeuriaImporter.CleanUp(Texture.Texture);
        }
        base.Removed();
    }
}

