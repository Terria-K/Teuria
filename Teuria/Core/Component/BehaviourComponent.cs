using Microsoft.Xna.Framework;

namespace Teuria;

public abstract class BehaviourComponent : Component 
{
    public Vector2 Position;
    public Vector2 GlobalPosition 
    {
        get => Entity.Position + Position;
    }

    public override sealed void Added(Entity entity)
    {
        base.Added(entity);
        Start();
    }

    public abstract void Start(); 

    public T GetComponent<T>() where T : Component
    {
        return Entity.GetComponent<T>();
    }
}