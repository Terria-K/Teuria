using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class PhysicsCanvas : CanvasLayer
{
    private readonly HashSet<PhysicsComponent> physicsComponents = new HashSet<PhysicsComponent>();
    private readonly QuadTree<PhysicsComponent> quadTree;
    private Texture2D Quad;
    private bool showDebug;
    private bool isClearing;

    public PhysicsCanvas(AABB bounds, bool showDebug = false) 
    {
        this.showDebug = showDebug;
        quadTree = new QuadTree<PhysicsComponent>(0, bounds);
    }

    public void Add(IPhysicsEntity entity) 
    {
        entity.Collider.IsInTheWorld = true;
        Add(entity.PhysicsComponent);
    }

    public void Add(PhysicsComponent component) 
    {
        physicsComponents.Add(component);
    }

    public void UpdatePhysics() 
    {
        if (isClearing) return;
        quadTree.Clear();
        quadTree.Insert(physicsComponents);
        foreach (var physicsComponent in physicsComponents) 
        {
            if (physicsComponent.Entity == null) 
            {
                physicsComponents.Remove(physicsComponent);
                continue;
            }
            var total = quadTree.Retrieve(physicsComponent);
            physicsComponent.Detect(new HashSet<PhysicsComponent>(total));
        }
    }

    public void ClearAll() 
    {
        isClearing = true;
        quadTree.Clear();
        physicsComponents.Clear();
        isClearing = false;
    }

    public override void Ready()
    {
#if DEBUG
        Quad = new Texture2D(SpriteBatch.GraphicsDevice, 1, 1);
        Quad.SetData(new Color[] { Color.Yellow });
#endif
        base.Ready();
    }

    public override void Draw()
    {
#if DEBUG
        if (showDebug)
            quadTree.ShowBoundaries(SpriteBatch, Quad, Color.White);
#endif
    }

    public override void Unload()
    {
        Quad.Dispose();
        base.Unload();
    }
}
