using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class PrototypeCanvas : CanvasLayer
{
    public Camera Camera;
    public Effect Effect;
    public Color EnvironmentColor;
    public SamplerState SamplerState = SamplerState.PointClamp;

    public PrototypeCanvas(Scene scene) 
    {
        this.Scene = scene;
    }

    public PrototypeCanvas(Scene scene, Camera camera) 
    {
        this.Scene = scene;
        this.Camera = camera;
    }

    public PrototypeCanvas(Scene scene, Camera camera, Color environemntColor, Effect effect = null) 
    {
        this.Scene = scene;
        this.Camera = camera;
        this.EnvironmentColor = environemntColor;
        Effect = effect;
    }

    public void ChangeScene(Scene scene) 
    {
        this.Scene = scene;
        GameApp.Instance.Scene = scene;
    }

    public override void Draw(Scene scene) 
    {
        Canvas.SpriteBatch.Begin(
#if FNA
            transformationMatrix: Camera.Transform, 
            rasterizerState: RasterizerState.CullCounterClockwise,
            depthStencilState: DepthStencilState.None,
#else
            // transformMatrix: Camera?.Transform, 
#endif
            samplerState: SamplerState, 
            sortMode: SpriteSortMode.Immediate, 
            blendState: BlendState.AlphaBlend,
            effect: Effect
        );
        Scene.Entities.Draw(Canvas.SpriteBatch);
        Canvas.SpriteBatch.End();
    }
}