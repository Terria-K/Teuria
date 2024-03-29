using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class AnimatedSprite : Component
{
    public float FPS { get; set; } = 0.09f;
    public bool IsFinished { get; private set; } = true;
    public bool Loop { get; set; }
    public string Animation => currentAnimation;
    public Spritesheet Atlas => atlas;
    public SpriteEffects SpriteEffects => spriteEffects;

    
    public float Rotation { get; set; }
    public Vector2 Scale { get; set; }
    private float timer;
    private Spritesheet atlas;
    private Dictionary<string, SFCyclesFrame> cycleFrame;
    private string currentAnimation = string.Empty;
    private int index;
    private int frameIndex;
    private Vector2 position;
    private SpriteEffects spriteEffects = SpriteEffects.None;


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

    public SpriteTexture Texture => atlas.Texture;
    

    public Vector2 Position 
    {
        get => position;
        set => position += value;
    }

    public Vector2 GlobalPosition 
    { 
        get 
        {
            if (Entity != null)
                return position + Entity.Transform.Position;
            throw new EntityDoesNotExistException();
        } 
        set 
        {
            if (Entity != null) 
            {
                position = Entity.Transform.Position + value;
                return;
            }

            throw new EntityDoesNotExistException();
        }
    }

    public int Frame 
    {
        get => frameIndex;
        set => frameIndex = value;
    }
    public int Width { get; private set; }
    public int Height { get; private set; }

    // Since SpriteFrameLoader is readonly ref struct, we can use 'in' here
    internal AnimatedSprite(scoped in SpriteFrameLoader loader) 
    {
        atlas = loader.Sheet;
        cycleFrame = loader.CycleFrame;
        Scale = Vector2.One;
        Width = Texture.Width;
        Height = Texture.Height;
    }

    public AnimatedSprite(string path, SpriteTexture texture) : this(new SpriteFrameLoader(path, texture)) {}
    public AnimatedSprite(string path, Atlas atlasTexture) : this(new SpriteFrameLoader(path, atlasTexture)) {}
    public AnimatedSprite(string path, ContentManager content) : this(new SpriteFrameLoader(path, content)) {}

    public void Play(string animationName) 
    {
        if (animationName == currentAnimation)
            return;
        currentAnimation = animationName;
        index = 0;
        timer = 0f;
        IsFinished = false;
    }

    public void Stop() 
    {
        IsFinished = true;
        timer = 0f;
    }

    public override void Update() 
    {
        UpdateAnimation();
        base.Update();
    }

    private void UpdateAnimation()
    {
        if (IsFinished)
            return;
        timer += Time.Delta;

        if (string.IsNullOrEmpty(currentAnimation) || timer <= FPS)
        {
            return;
        }
        var animation = cycleFrame[currentAnimation];
        frameIndex = animation.Frames[index];
        timer = 0f;
        index++;

        if (index < animation.Frames.Length)
        {
            return;
        }
        if (animation.IsLooping || Loop)
        {
            index = 0;
            return;
        }
        Stop();
    }

    public SpriteTexture GetSpriteTexture(int id) => atlas[frameIndex];

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Entity != null) 
        {
            atlas[frameIndex]?.DrawTexture(
                spriteBatch, GlobalPosition, Entity.Modulate, Rotation, Scale, spriteEffects, Entity.ZIndex);
        }

        base.Draw(spriteBatch);
    }
}