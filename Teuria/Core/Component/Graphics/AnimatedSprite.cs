using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class AnimatedSprite : Component
{
    public float FPS { get; set; } = 0.09f;
    public bool IsFinished { get; private set; } = true;
    public bool Loop { get; set; }
    public string Animation { get => currentAnimation;  }
    public float Rotation { get; set; }
    public Vector2 Scale { get; set; }
    private float timer;
    private TextureAtlas atlas;
    private Dictionary<string, SFCyclesFrame> cycleFrame;
    private string currentAnimation;
    private int index;
    private int frameIndex;
    private Vector2 position;
    private SpriteEffects spriteEffects = SpriteEffects.None;

    public TextureAtlas Atlas => atlas;
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

    public SpriteTexture Texture => atlas.Texture;
    

    public Vector2 Position 
    {
        get => position;
        set => position += value;
    }

    public Vector2 GlobalPosition 
    { 
        get => position + Entity.Position;
        set 
        {
            position = Entity.Position + value;
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
    public AnimatedSprite(in SpriteFrameLoader loader) 
    {
        atlas = loader.Atlas;
        cycleFrame = loader.CycleFrame;
        Scale = Vector2.One;
        Width = Texture.Width;
        Height = Texture.Height;
    }

    public AnimatedSprite(string path, SpriteTexture texture) : this(new SpriteFrameLoader(path, texture)) {}

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

        if (timer <= FPS)
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
        atlas[frameIndex].DrawTexture(spriteBatch, GlobalPosition, Entity.Modulate, Rotation, Scale, spriteEffects, Entity.ZIndex);
        base.Draw(spriteBatch);
    }
}