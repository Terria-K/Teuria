using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TeuJson;

namespace Teuria;

public class Entity : Node, IEnumerable<Component>
{
    private List<Component> componentList = new List<Component>();
    public IReadOnlyList<Component> Components => componentList;
    public Transform Transform = new Transform();
    public Vector2 Position 
    {
        get => Transform.Position;
        set => Transform.Position = value;
    }
    public float PosX
    {
        get => Transform.PosX;
        set => Transform.PosX = value;
    }

    public float PosY
    {
        get => Transform.PosY;
        set => Transform.PosY = value;
    }

    public Vector2 LocalPosition 
    {
        get => Transform.LocalPosition;
        set => Transform.LocalPosition = value;
    }

    public float Rotation 
    {
        get => Transform.Rotation;
        set => Transform.Rotation = value;
    }

    public Vector2 Scale 
    {
        get => Transform.Scale;
        set => Transform.Scale = value;
    }
    public Color Modulate = Color.White;
    public float ZIndex;
    public int Depth;
    public int Tags;
    public bool Visible = true;
    public SpriteEffects SpriteEffects;
    public bool FlipH
    {
        get => (SpriteEffects & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally;
        
        set => SpriteEffects = value 
                ? SpriteEffects | SpriteEffects.FlipHorizontally 
                : SpriteEffects & ~SpriteEffects.FlipHorizontally;
        
    }
    public bool FlipV
    {
        get => (SpriteEffects & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically;
        set => SpriteEffects = value 
                ? SpriteEffects | SpriteEffects.FlipVertically 
                : SpriteEffects & ~SpriteEffects.FlipVertically;
    }

    public override void EnterScene(Scene scene, ContentManager content) 
    {
        foreach (var comp in componentList) 
            comp.EntityEntered(scene);
        
        base.EnterScene(scene, content);
    }
    public override void ExitScene(Scene scene) 
    {
        foreach (var comp in componentList) 
        {
            comp.Removed();
            comp.EntityExited(scene);
        }
        base.ExitScene(scene);
    }
    public override void Ready() {}
    public override void Update() 
    {
        for (int i = 0; i < componentList.Count; i++) 
        {
            if (!componentList[i].Active) continue;
            componentList[i].Update();
        }
    }

    public override void Draw(SpriteBatch spriteBatch) 
    {
        if (!Visible) return;
        for (int i = 0; i < componentList.Count; i++) 
        {
            if (!componentList[i].Active) continue;
            componentList[i].Draw(spriteBatch);
        }
    }

    public void AddTransform(Entity entity, bool stay = false) 
    {
        entity.Transform.SetParent(Transform, stay);
    }

    public void AddTransform(Transform transform, bool stay = false) 
    {
        transform.SetParent(Transform, stay);
    }

    public void AddComponent(Component comp) 
    {
        componentList.Add(comp);
        comp.Added(this);
    }

    public void AddComponent<T>(T[] comps) 
    where T : Component
    {
        foreach (var comp in comps) 
        {
            AddComponent(comp);
        }
    }


    public T? GetComponent<T>() where T : Component
    {
        Span<Component> comps = CollectionsMarshal.AsSpan(componentList);
        foreach (var comp in comps) 
        {
            if (comp is T c) 
            {
                return c;
            }
        }
    
        return default;
    }

    [Conditional("DEBUG")]
    public void AssertComponent<T>() where T : Component 
    {
        var component = GetComponent<T>();
        SkyLog.Assert(component != null, $"This entity does not have {component} Component!");
    }

    public void RemoveComponent(Component comp) 
    {
        if (comp == null)
            return;
        comp.Removed();
        componentList.Remove(comp);
    }

    public void DestroySelf() 
    {
        Scene?.Remove(this);
    }

    public IEnumerator<Component> GetEnumerator() => componentList.GetEnumerator();
    

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public virtual JsonValue Import() 
    {
        var jsonObj = new JsonObject() 
        {
            ["position"] = new JsonObject 
            {
                ["x"] = PosX,
                ["y"] = PosY
            },
            ["localPosition"] = new JsonObject 
            {
                ["x"] = LocalPosition.X,
                ["y"] = LocalPosition.Y
            },
            ["rotation"] = Rotation,
            ["scale"] = new JsonObject 
            {
                ["x"] = Scale.X,
                ["y"] = Scale.Y
            },
            ["modulate"] = new JsonObject 
            {
                ["r"] = Modulate.R,
                ["g"] = Modulate.G,
                ["b"] = Modulate.B,
                ["a"] = Modulate.A
            },
            ["flipH"] = FlipH,
            ["flipV"] = FlipV,
            ["depth"] = Depth,
            ["tags"] = Tags,
            ["visible"] = Visible,
            ["active"] = Active
        };
        return jsonObj;
    }

    public virtual void Export(JsonValue val) 
    {
        var position = val["position"];
        Position = new Vector2(position["x"], position["y"]);

        var localPosition = val["localPosition"];
        LocalPosition = new Vector2(localPosition["x"], localPosition["y"]);

        Rotation = val["rotation"];
        var scale = val["scale"];
        Scale = new Vector2(scale["x"], scale["y"]);

        var modulate = val["modulate"];
        Modulate = new Color(modulate["r"], modulate["g"], modulate["b"], modulate["a"]);

        FlipH = val["flipH"];
        FlipV = val["flipV"];
        Depth = val["depth"];
        Tags = val["tags"];
        Visible = val["visible"];
        Active = val["active"];
    }
}