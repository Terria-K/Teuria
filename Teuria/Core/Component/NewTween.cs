using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Teuria.Tweening;


public sealed class Sequence : Component
{
    private Stack<Tweener> tweenerStacks = new();
    private Tweener? tweener;
    private bool destroyOnFinished;

    private Sequence() 
    {
        Active = false;
    }

    public static Sequence Create(Entity entity) 
    {
        var sequence = new Sequence();
        entity.AddComponent(sequence);
        return sequence;
    }

    public Sequence Append(Tweener tweener) 
    {
        tweenerStacks.Push(tweener);
        return this;
    }

    public Sequence DestroyOnFinished(bool destroy) 
    {
        destroyOnFinished = destroy;
        return this;
    }

    public void Destroy() 
    {
        if (Entity != null)
            DetachSelf();
    }

    public override void Update()
    {
        if (tweenerStacks.Count <= 0) 
        {
            Active = false;
            base.Update();
            return;
        }
        if (tweener == null) 
        {
            tweener = tweenerStacks.Pop();
            tweener.Play();
        }
        else if (!tweener.Active) 
        {
            tweener = tweenerStacks.Pop();
            tweener.Play();
        }


        base.Update();
    }

    public void Play(Entity entity) 
    {
        entity.AddComponent(this);
        Active = true;
    }
}

public sealed class Tweener 
{
    internal Tween TweenComponent;
    internal Action<Tween>? OnReadyCallback;
    internal Action<Tween>? OnUpdateCallback;
    internal Action<Tween>? OnCompletedCallback;

    public bool Active => TweenComponent.TimeLeft > 0;
    
    
    internal Tweener(Entity entity) 
    {
        TweenComponent = Tween.Create(entity, Tween.TweenMode.OneShot, null);
    }


    public Tweener SetEase(Ease.Easer easing) 
    {
        TweenComponent.Easer = easing;
        return this;
    }

    public Tweener SetDelay(float delay) 
    {
        TweenComponent.Delay = delay;
        return this;
    }

    public Tweener OnReady(Action<Tween> tween) 
    {
        this.OnReadyCallback = tween;
        return this;
    }

    public Tweener OnUpdate(Action<Tween> tween) 
    {
        this.OnUpdateCallback = tween;
        return this;
    }

    public Tweener OnCompleted(Action<Tween> tween) 
    {
        this.OnCompletedCallback = tween;
        return this;
    }

    public Tweener Play() 
    {
        TweenComponent.Start();
        return this;
    }

    public IEnumerator Wait() 
    {
#if DEBUG
        if (!TweenComponent.Active) 
        {
            SkyLog.Log("Waited while the Tween is not running.", SkyLog.LogLevel.Assert);
            SkyLog.Log("Make sure to play the tween first.", SkyLog.LogLevel.Warning);
        }
#endif
        yield return TweenComponent.Wait();
    }

    public Tweener SetMode(Tween.TweenMode mode) 
    {
        TweenComponent.Mode = mode;
        return this;
    }

    public void Destroy() 
    {
        TweenComponent.DetachSelf();
    }

    internal void AddToTween(Action<Tween> tween, float duration) 
    {
        TweenComponent.Duration = duration;
        TweenComponent.OnReady = t => 
        {
            OnReadyCallback?.Invoke(t);
        };
        TweenComponent.OnProcess = t => 
        {
            tween(t);
            OnUpdateCallback?.Invoke(t);
        };
        TweenComponent.OnEnd = t => 
        {
            OnCompletedCallback?.Invoke(t);
        };
    }
}

public static class TweenUtils 
{
#region Rotation
    public static Tweener TWRotation(this Entity entity, float targetRotation, float duration) 
    {
        var ctx = new Tweener(entity);
        var cachedRotation = entity.Rotation;
        ctx.AddToTween(t => 
        {
            entity.Rotation = MathHelper.Lerp(cachedRotation, targetRotation, t.Value);
        }, duration);
        return ctx;
    }
#endregion
#region GlobalMove
    public static Tweener TWGlobalMove(this Entity entity, Vector2 targetPosition, float duration) 
    {
        var ctx = new Tweener(entity);
        var cachedPosition = entity.Position;
        ctx.AddToTween(t => 
        {
            // entity.Position = cachedPosition + (targetPosition - cachedPosition) * t.Value;
            entity.Position = Vector2.Lerp(cachedPosition, targetPosition, t.Value);
        }, duration);
        return ctx;
    }

    public static Tweener TWGlobalMoveX(this Entity entity, float targetPosition, float duration) 
    {
        var ctx = new Tweener(entity);
        var cachedPosition = entity.Position;
        ctx.AddToTween(t => 
        {
            entity.Position = Vector2.Lerp(cachedPosition, new Vector2(targetPosition, entity.PosY), t.Value);
        }, duration);
        return ctx;
    }

    public static Tweener TWGlobalMoveY(this Entity entity, float targetPosition, float duration)
    {
        var ctx = new Tweener(entity);
        var cachedPosition = entity.Position;
        ctx.AddToTween(t => 
        {
            entity.Position = Vector2.Lerp(cachedPosition, new Vector2(entity.PosX, targetPosition), t.Value);
        }, duration);
        return ctx;
    }
#endregion

#region Move
    public static Tweener TWMove(this Entity entity, Vector2 targetPosition, float duration) 
    {
        var ctx = new Tweener(entity);
        var cachedPosition = entity.LocalPosition;
        ctx.AddToTween(t => 
        {
            entity.LocalPosition = Vector2.Lerp(cachedPosition, targetPosition, t.Value);
        }, duration);
        return ctx;
    }

    public static Tweener TWMoveX(this Entity entity, float targetPosition, float duration) 
    {
        var ctx = new Tweener(entity);
        var cachedPosition = entity.LocalPosition;
        ctx.AddToTween(t => 
        {
            entity.LocalPosition = Vector2.Lerp(
                cachedPosition, 
                new Vector2(targetPosition, entity.LocalPosition.Y), 
                t.Value
            );
        }, duration);
        return ctx;
    }

    public static Tweener TWMoveY(this Entity entity, float targetPosition, float duration) 
    {
        var ctx = new Tweener(entity);
        var cachedPosition = entity.LocalPosition;
        ctx.AddToTween(t => 
        {
            entity.LocalPosition = Vector2.Lerp(
                cachedPosition, 
                new Vector2(entity.LocalPosition.X, targetPosition), 
                t.Value
            );
        }, duration);
        return ctx;
    }
#endregion

#region Modulate
    public static Tweener TWModulate(this Entity entity, Color targetModulate, float duration) 
    {
        var ctx = new Tweener(entity);
        var cachedColor = entity.Modulate;
        ctx.AddToTween(t => 
        {
            entity.Modulate = Color.Lerp(cachedColor, targetModulate, t.Value);
        }, duration);
        return ctx;
    }

#endregion

#region Camera
    public static Tweener TWCameraZoom(this Entity entity, Camera camera, float zoomTo, float duration) 
    {
        var ctx = new Tweener(entity);
        var cachedZoom = camera.Zoom;
        ctx.AddToTween(t => 
        {
            camera.Zoom = Vector2.Lerp(cachedZoom, new Vector2(zoomTo), t.Value);
        }, duration);
        return ctx;
    }

    public static Tweener TWCameraMove(this Entity entity, Camera camera, Vector2 targetPosition, float duration) 
    {
        var ctx = new Tweener(entity);
        var cachedPosition = camera.Position;
        ctx.AddToTween(t => 
        {
            camera.Position = Vector2.Lerp(cachedPosition, targetPosition, t.Value);
        }, duration);
        return ctx;
    }

    public static Tweener TWCameraMoveX(this Entity entity, Camera camera, float targetPosition, float duration) 
    {
        var ctx = new Tweener(entity);
        var cachedPosition = camera.Position;
        ctx.AddToTween(t => 
        {
            camera.Position = Vector2.Lerp(cachedPosition, new Vector2(targetPosition, camera.Position.Y), t.Value);
        }, duration);
        return ctx;
    }

    public static Tweener TWCameraMoveY(this Entity entity, Camera camera, float targetPosition, float duration) 
    {
        var ctx = new Tweener(entity);
        var cachedPosition = camera.Position;
        ctx.AddToTween(t => 
        {
            camera.Position = Vector2.Lerp(cachedPosition, new Vector2(camera.Position.X, targetPosition), t.Value);
        }, duration);
        return ctx;
    }
#endregion

    public static Tweener TWCustom(this Entity entity, Action<Tween> onProcess, float duration) 
    {
        var ctx = new Tweener(entity);
        ctx.AddToTween(t => 
        {
            onProcess(t);
        }, duration);
        return ctx;
    }
}