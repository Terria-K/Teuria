using System.Collections.Generic;

namespace Teuria;

public interface IPhysicsEntity 
{
    Collider Collider { get; set; }
    AABB BoundingArea { get; }

    void Detect(HashSet<IPhysicsEntity> entity);
}