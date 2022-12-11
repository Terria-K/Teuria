using System.Collections.Generic;

namespace Teuria;

public interface ICollidableEntity 
{
    Shape Collider { get; }
    PhysicsComponent PhysicsComponent { get; }
}