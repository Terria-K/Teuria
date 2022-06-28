using System.Collections.Generic;

namespace Teuria;

public interface IPhysicsEntity 
{
    Collider Collider { get; }
    PhysicsComponent PhysicsComponent { get; }
}