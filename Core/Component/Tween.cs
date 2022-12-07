using System;
using System.Collections;
using Microsoft.Xna.Framework;

namespace Teuria;

// https://github.com/JamesMcMahon/monocle-engine/blob/759c15ad6a59c4855db36fd0dfc1970cef9607a5/Monocle/Components/Logic/Tween.cs
// https://github.com/JamesMcMahon/monocle-engine/blob/759c15ad6a59c4855db36fd0dfc1970cef9607a5/Monocle/Util/Ease.cs

public class Tween : Component
{
    public enum TweenMode { Persistent, OneShot, Loop }

    public TweenMode Mode { get; private set; }
    public Ease.Easer Easer { get; private set; }
    public float Duration { get; private set; }
    public float TimeLeft { get; private set; }
    public float Progress { get; private set; }
    public float Value { get; private set; }
    public bool Reverse { get; private set; }

    public Action<Tween> OnReady;
    public Action<Tween> OnProcess;
    public Action<Tween> OnEnd;


    public Tween(TweenMode mode, Ease.Easer easer = null, float duration = 1f, bool start = false) 
    {
        Mode = mode;
        Easer = easer;
        Duration = duration;

        TimeLeft = 0;
        Progress = 0;
        Active = false;
    }

    public override void Added(Entity entity)
    {
        Entity = entity;
    }

    public override void Update()
    {
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
                    Start(Reverse);
                    break;
            }
        }
        base.Update();
    }

    public void Start(float duration, bool reverse = false) 
    {  
#if DEBUG
        if (duration <= 0) { throw new Exception("Infinite Tween detected! Duration cannot be less than 0"); }
#endif
        Duration = duration;
        Start(reverse);
    } 

    public void Start(bool reverse = false) 
    {
        Reverse = reverse;
        TimeLeft = Duration;
        Value = Progress = Reverse ? 1 : 0;

        Active = true;
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