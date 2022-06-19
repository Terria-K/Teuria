using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class SceneRenderer : Renderer
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

    public SceneRenderer(Scene scene, Camera camera, Color environemntColor) 
    {
        this.Scene = scene;
        this.Camera = camera;
        this.EnvironmentColor = environemntColor;
        this.Scene.SceneRenderer = this;
    }

    public void ChangeScene(Scene scene) 
    {
        this.Scene = scene;
        this.Scene.SceneRenderer = this;
        TeuriaEngine.Instance.Scene = scene;
    }

    public override void Draw() 
    {
        SpriteBatch.Begin(transformMatrix: Camera?.Transform, sortMode: SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend);
        Scene.Draw(SpriteBatch);
        SpriteBatch.End();
    }
}