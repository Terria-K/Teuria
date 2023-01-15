using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class Entities : IEnumerable<Entity>
{
    public Scene Scene { get; private set; }

    private List<Entity> entities = new List<Entity>();
    private List<Entity> add = new List<Entity>();
    private List<Entity> remove = new List<Entity>();
    private List<Entity> entering = new List<Entity>();
    private HashSet<Entity> current = new HashSet<Entity>();
    private HashSet<Entity> adding = new HashSet<Entity>();
    private bool unsorted;

    public int Count => entities.Count;
    internal Entities(Scene scene) 
    {
        Scene = scene;
    }

    public Entity this[int idx] 
    {
        get 
        {
            if (idx < 0 || idx >= entities.Count) { throw new IndexOutOfRangeException(); }
            return entities[idx];
        }    
    }

    public void UpdateSystem() 
    {
        if (add.Count > 0) 
        {
            for (int i = 0; i < add.Count; i++) 
            {
                var entity = add[i];
                if (!current.Contains(entity)) 
                {
                    current.Add(entity);
                    entities.Add(entity);
                    if (Scene == null) { continue; }
                    entity.EnterScene(Scene, Scene.GetContent());
                }
            }
            unsorted = true;
        }

        if (remove.Count > 0) 
        {
            for (int i = 0; i < remove.Count; i++) 
            {
                var entity = remove[i];
                if (entities.Contains(entity)) 
                {
                    current.Remove(entity);
                    entities.Remove(entity);

                    if (Scene == null) { continue; }
                    entity.ExitScene();
                }
            }
            remove.Clear();
        }

        if (unsorted) 
        {
            unsorted = false;
            entities.Sort(CompareDepth);

        }

        if (add.Count > 0) 
        {
            entering.AddRange(add);
            add.Clear();
            adding.Clear();
            foreach (var entity in entering) 
            {
                if (entity.Scene == Scene) 
                {
                    entity.Ready();
                }
            }
            entering.Clear();
        }
    }

    public void Add(Entity entity) 
    {
        if (!adding.Contains(entity) && !current.Contains(entity)) 
        {
            adding.Add(entity);
            add.Add(entity);
        }
    }

    public void Remove(Entity entity) 
    {
        remove.Add(entity);
    }

    public void Clear() 
    {
        foreach (var entity in entities) 
        {
            remove.Add(entity);
        }
    }

    internal void Update() 
    {
        foreach (var entity in entities)
        {
            if (!entity.Active)
                continue;
            
            if (Scene.Paused && entity.PauseMode != PauseMode.Single)
                continue;

            entity.Update();
        }
    }

    public void Draw(SpriteBatch spriteBatch) 
    {
        foreach (var entity in entities) 
        {
            if (entity.Visible) { entity.Draw(spriteBatch); }
        }
    }

    public IEnumerator<Entity> GetEnumerator()
    {
        return entities.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static Comparison<Entity> CompareDepth = (a, b) => Math.Sign(b.Depth - a.Depth);
}