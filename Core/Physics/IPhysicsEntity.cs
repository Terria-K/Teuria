using System.Collections.Generic;

namespace Teuria;

public interface IPhysicsEntity 
{
    Shape Collider { get; }
    PhysicsComponent PhysicsComponent { get; }
}