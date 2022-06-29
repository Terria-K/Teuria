using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class AnimatedSprite : Component
{
    public float FPS { get; set; } = 0.09f;
    public bool IsFinished { get; private set; } = true;
    public string Animation { get => currentAnimation;  }
    private SpriteFrameLoader frameLoader;
    private float timer;
    private string currentAnimation;
    private int index;
    private int frameCount;
    private int frameIndex;

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
    private Vector2 position;

    public AnimatedSprite(SpriteFrameLoader loader, int frameCount) 
    {
        frameLoader = loader;
        this.frameCount = frameCount;
    }

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
        timer += TeuriaEngine.DeltaTime;

        if (timer <= FPS)
        {
            return;
        }
        var animation = frameLoader.CycleFrame[currentAnimation];
        frameIndex = animation.Frames[index];
        timer = 0f;
        index++;

        if (index < animation.Frames.Length)
        {
            return;
        }
        if (animation.IsLooping)
        {
            index = 0;
            return;
        }
        Stop();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        frameLoader.Atlas[frameIndex].DrawTexture(spriteBatch, GlobalPosition, Color.White, 0f, Vector2.One, SpriteEffects.None, 1);
        base.Draw(spriteBatch);
    }
}