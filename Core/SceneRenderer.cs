using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class SceneCanvas : CanvasLayer
{
    private Camera Camera;
    public Color EnvironmentColor;
    public SamplerState SamplerState = SamplerState.PointClamp;

    public SceneCanvas(Scene scene, SpriteBatch spriteBatch) 
    {
        this.Scene = scene;
        this.Scene.MainCanvas = this;
    }

    public SceneCanvas(Scene scene, Camera camera, SpriteBatch spriteBatch) 
    {
        this.Scene = scene;
        this.Camera = camera;
        this.Scene.MainCanvas = this;
    }

    public SceneCanvas(Scene scene, Camera camera, Color environemntColor) 
    {
        this.Scene = scene;
        this.Camera = camera;
        this.EnvironmentColor = environemntColor;
        this.Scene.MainCanvas = this;
    }

    public void ChangeScene(Scene scene) 
    {
        this.Scene = scene;
        this.Scene.MainCanvas = this;
        TeuriaEngine.Instance.Scene = scene;
    }

    public override void Draw() 
    {
        SpriteBatch.Begin(transformMatrix: Camera?.Transform, samplerState: SamplerState, sortMode: SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend);
        Scene.Render();
        SpriteBatch.End();
    }
}