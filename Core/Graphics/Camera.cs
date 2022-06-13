using Microsoft.Xna.Framework;

namespace Teuria;

public class Camera : Entity
{
    public Matrix Transform { get; private set; }
    public void Follow(Entity target, bool smooth = false)
    {
        Matrix offset = Matrix.CreateTranslation(
                TeuriaEngine.screenWidth / 2,
                TeuriaEngine.screenHeight / 2,
                0);

        Matrix position = Matrix.CreateTranslation(
                -target.Position.X - (target.Rectangle.Width / 2),
                -target.Position.Y - (target.Rectangle.Height / 2),
                        0);
        Transform = position * offset;
    }
}

