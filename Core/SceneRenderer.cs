using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class SceneCanvas : CanvasLayer
{
    public Camera Camera;
    public Effect Effect;
    public Color EnvironmentColor;
    public SamplerState SamplerState = SamplerState.PointClamp;

    public SceneCanvas(Scene scene) 
    {
        this.Scene = scene;
        this.Scene.MainCanvas = this;
    }

    public SceneCanvas(Scene scene, Camera camera) 
    {
        this.Scene = scene;
        this.Camera = camera;
        this.Scene.MainCanvas = this;
    }

    public SceneCanvas(Scene scene, Camera camera, Color environemntColor, Effect effect = null) 
    {
        this.Scene = scene;
        this.Camera = camera;
        this.EnvironmentColor = environemntColor;
        this.Scene.MainCanvas = this;
        Effect = effect;
    }

    public void ChangeScene(Scene scene) 
    {
        this.Scene = scene;
        this.Scene.MainCanvas = this;
        TeuriaEngine.Instance.Scene = scene;
    }

    public override void Draw(Scene scene) 
    {
        Canvas.SpriteBatch.Begin(
            transformMatrix: Camera?.Transform, 
            samplerState: SamplerState, 
            sortMode: SpriteSortMode.Immediate, 
            blendState: BlendState.AlphaBlend,
            effect: Effect
        );
        Scene.Entities.Draw(Canvas.SpriteBatch);
        Canvas.SpriteBatch.End();
    }
}