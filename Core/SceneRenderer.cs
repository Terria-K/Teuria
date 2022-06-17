using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class SceneRenderer 
{
    private Camera Camera;
    private Scene Scene;
    public Color EnvironmentColor;

    public SceneRenderer(Scene scene) 
    {
        this.Scene = scene;
        this.Scene.SceneRenderer = this;
    }

    public SceneRenderer(Scene scene, Camera camera) 
    {
        this.Scene = scene;
        this.Camera = camera;
        this.Scene.SceneRenderer = this;
    }

    public void ChangeScene(Scene scene) 
    {
        this.Scene = scene;
        this.Scene.SceneRenderer = this;
        TeuriaEngine.Instance.Scene = scene;
    }

    public void Draw(SpriteBatch spriteBatch) 
    {
        spriteBatch.Begin(transformMatrix: Camera?.Transform);
        Scene.Draw(spriteBatch);
        spriteBatch.End();
    }
}