using Microsoft.Xna.Framework;

namespace Teuria;

public interface IPhysicsEntity 
{
    Hitbox Hitbox { get; }
    AABB BoundingArea { get; }

    bool Collide(Hitbox hitbox);

    bool Collide(Rectangle boundingBox);
}