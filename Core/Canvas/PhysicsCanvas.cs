using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class PhysicsCanvas : CanvasLayer
{
    private readonly HashSet<IPhysicsEntity> entities;
    private readonly QuadTree<IPhysicsEntity> quadTree;
    private Texture2D Quad;
    private bool showDebug;

    public PhysicsCanvas(AABB bounds, bool showDebug = false) 
    {
        this.showDebug = showDebug;
        quadTree = new QuadTree<IPhysicsEntity>(0, bounds);
        entities = new HashSet<IPhysicsEntity>();
    }

    public void Add(IPhysicsEntity entity) 
    {
        entities.Add(entity);
    }

    public void UpdatePhysics() 
    {
        quadTree.Clear();
        quadTree.Insert(entities);
        foreach (var entity in entities) 
        {
            var total = quadTree.Retrieve(entity);
            entity.Detect(new HashSet<IPhysicsEntity>(total));
            // foreach (var retEntity in total) 
            // {
            //     if (entity.Collider.BoundingArea.Equals(retEntity.Collider.BoundingArea)) 
            //         { continue; }
            //     if (entity.Collider.BoundingArea.Contains(retEntity.Collider.BoundingArea)) 
            //     {
            //         entity.Detect(retEntity);
            //     }
            // }
        }
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
