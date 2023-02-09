using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
#if DEBUG
            if (idx < 0 || idx >= entities.Count) { throw new IndexOutOfRangeException(); }
#endif
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
                    Scene.OnEntityCreated?.Invoke(entity);
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
                    Scene.OnEntityDeleted?.Invoke(entity);
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

    internal void SortEntities() 
    {
        unsorted = true;
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


    public List<T> GetEntities<T>() 
    where T : Entity
    {
        var list = Enumerable.Empty<T>().ToList();
        foreach (var entity in entities) 
        {
            if (entity is T ent) 
                list.Add(ent);
        }

        foreach (var entity in adding) 
        {
            if (entity is T ent) 
                list.Add(ent);
        }
        return list;
    }

    public List<Entity> GetEntitiesByTag(int tags) 
    {
        var list = Enumerable.Empty<Entity>().ToList();
        foreach (var entity in entities) 
        {
            if ((entity.Tags & tags) != 0) 
            {
                list.Add(entity);
            }
        }

        foreach (var entity in adding) 
        {
            if ((entity.Tags & tags) != 0) 
            {
                list.Add(entity);
            }
        }
        return list;
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