using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Teuria;

// https://github.com/JamesMcMahon/monocle-engine/blob/759c15ad6a59c4855db36fd0dfc1970cef9607a5/Monocle/Components/Logic/Tween.cs
// https://github.com/JamesMcMahon/monocle-engine/blob/759c15ad6a59c4855db36fd0dfc1970cef9607a5/Monocle/Util/Ease.cs

public class Tween : Component
{
    public enum TweenMode { Persistent, OneShot, Loop, YoyoLoop }

    public TweenMode Mode { get; private set; }
    public Ease.Easer Easer { get; private set; }
    public float Duration { get; private set; }
    public float TimeLeft { get; private set; }
    public float Progress { get; private set; }
    public float Delay { get; private set; }
    public float Value { get; private set; }
    public bool Reverse { get; private set; }
    private float lastDelay;


    public Action<Tween> OnReady;
    public Action<Tween> OnProcess;
    public Action<Tween> OnEnd;

    private static Stack<Tween> cached = new Stack<Tween>();


    public Tween() {}

    public static Tween Create(Entity entity, TweenMode mode, Ease.Easer easer = null, float duration = 1f) 
    {
        Tween tween;
        if (cached.Count == 0)
            tween = new Tween();
        else
            tween = cached.Pop();
        tween.Mode = mode;
        tween.Easer = easer;
        tween.Duration = duration;
        tween.Active = false;
        
        entity.AddComponent(tween);
        return tween;
    }

    public override void Added(Entity entity)
    {
        Entity = entity;
    }

    public override void Update()
    {
        if (Delay > 0) 
        {
            Delay -= TeuriaEngine.DeltaTime;
            base.Update();
            return;
        }

        TimeLeft -= TeuriaEngine.DeltaTime;

        Progress = Math.Max(0, TimeLeft) / Duration;

        if (!Reverse) { Progress = 1 - Progress; }
        if (Easer != null) { Value = Easer(Progress); }
        else { Value = Progress; }

        OnProcess?.Invoke(this);

        if (TimeLeft <= 0) 
        { 
            TimeLeft = 0;

            OnEnd?.Invoke(this);
            switch (Mode) 
            {
                case TweenMode.Persistent:
                    Active = false;
                    break;
                case TweenMode.OneShot:
                    Active = false;
                    Entity.RemoveComponent(this);
                    break;
                case TweenMode.Loop:
                    Start(Reverse, lastDelay);
                    break;
                case TweenMode.YoyoLoop:
                    Start(!Reverse, lastDelay);
                    break;
            }
        }
        base.Update();
    }

    public void Start(float duration, bool reverse = false, float delay = 0f) 
    {  
#if DEBUG
        if (duration <= 0) { throw new Exception("Infinite Tween detected! Duration cannot be less than 0"); }
#endif
        Duration = duration;
        Start(reverse, delay);
    } 

    public void Start(bool reverse = false, float delay = 0f) 
    {
        Active = true;
        Reverse = reverse;
        TimeLeft = Duration;
        Value = Progress = Reverse ? 1 : 0;
        Delay = delay;
        lastDelay = delay;
        OnReady?.Invoke(this);
    }

    public IEnumerator Wait() 
    {
        while (Active)
            yield return null;
    }
}

public static class Ease 
{
    public delegate float Easer(float t);

    public static readonly Easer Linear = (float t) => t;

    public static readonly Easer SineIn = (float t) => -(float)Math.Cos(MathHelper.PiOver2 * t) + 1;
    public static readonly Easer SineOut = (float t) => (float)Math.Sin(MathHelper.PiOver2 * t);
    public static readonly Easer SineInOut = (float t) => -(float)Math.Cos(MathHelper.Pi * t) / 2f + .5f;
}