using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public interface IComponent 
{
    bool Active { get; set; }

    void Added(Entity entity);
    void Update();
    void Draw(SpriteBatch spriteBatch);
    void Removed();
}