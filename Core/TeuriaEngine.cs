using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class TeuriaEngine : Game
{
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private Scene scene;
    private Scene nextScene;
    public static float DeltaTime { get; private set; }
    public static TeuriaEngine Instance => instance;
    public Scene Scene 
    { 
        get => scene; 
        set 
        { 
            scene ??= value;
            nextScene = value; 
        } 
    }
    private static TeuriaEngine instance;
    private SceneRenderer renderer;

    public static int screenHeight;
    public static int screenWidth;

    public TeuriaEngine(int width, int height, int screenWidth, int screenHeight, string windowTitle, bool fullScreen)
    {
        instance = this;
        Window.Title = windowTitle;
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected virtual SceneRenderer Init() { throw new NotImplementedException(); }
    protected virtual void Load() {}
    protected virtual void Process(GameTime gameTime) {}
    protected virtual void CleanUp() {}
    protected virtual RenderProperties Render() { return default; }

    protected override void Initialize()
    {
        screenHeight = graphics.PreferredBackBufferHeight;
        screenWidth = graphics.PreferredBackBufferWidth;
        renderer = Init();
        scene.Initialize();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        Load();
        
        scene.Ready(graphics.GraphicsDevice);
    }

    protected override void UnloadContent()
    {
        CleanUp();
        base.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {   
        DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Process(gameTime);
        
        if (scene != nextScene) 
        {
            scene.Exit();
            scene = nextScene;
            scene?.Ready(graphics.GraphicsDevice);
        }
        scene.Update();


        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(renderer.EnvironmentColor);
        renderer.Draw(spriteBatch);
        // spriteBatch.Begin(transformMatrix: renderProperties.Camera);
        // scene.Draw(spriteBatch);
        // spriteBatch.End();

        base.Draw(gameTime);
    }

    public void ExitGame() 
    {
        Exit();
    }
}

