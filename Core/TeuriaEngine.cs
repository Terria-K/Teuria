using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Teuria;

public class TeuriaEngine : Game
{
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private Scene scene;
    private Scene nextScene;
    private int fpsCounter;
    private TimeSpan counterElapsed = TimeSpan.Zero;
    private string title;
    
    public static int FPS { get; private set; }
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

    public static int ScreenHeight;
    public static int ScreenWidth;

    public TeuriaEngine(int width, int height, int screenWidth, int screenHeight, string windowTitle, bool fullScreen)
    {
        instance = this;
        Window.Title = windowTitle;
        title = windowTitle;
        // ScreenHeight = screenHeight;
        // ScreenWidth = screenWidth;
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
        // SetFullscreen(fullScreen);
    }

    private void SetFullscreen(bool fullscreen) 
    {
        if (fullscreen) 
        {
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.IsFullScreen = true;
            return;
        }
        graphics.PreferredBackBufferHeight = ScreenWidth;
        graphics.PreferredBackBufferHeight = ScreenHeight;
        graphics.IsFullScreen = false;
    }

    protected virtual SceneRenderer Init() { throw new NotImplementedException(); }
    protected virtual void Load() {}
    protected virtual void Process(GameTime gameTime) {}
    protected virtual void CleanUp() {}
    protected virtual RenderProperties Render() { return default; }

    protected override void Initialize()
    {
        ScreenHeight = graphics.PreferredBackBufferHeight;
        ScreenWidth = graphics.PreferredBackBufferWidth;
        renderer = Init();
        scene.Initialize();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        Canvas.Initialize(graphics.GraphicsDevice);
        TInput.Initialize();
        spriteBatch = new SpriteBatch(GraphicsDevice);
        Load();
        
        scene.Ready(graphics.GraphicsDevice);
    }

    protected override void UnloadContent()
    {
        Scene.Exit();
        CleanUp();
        foreach(var textures in TextureImporter.cleanupCache) 
        {
            textures?.Dispose();
        }
        base.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {   
        DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        TInput.Update();
        Process(gameTime);

#if DEBUG
        Hitbox.DebugRender = Keyboard.GetState().IsKeyDown(Keys.F1);
#endif
        
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

        base.Draw(gameTime);

        fpsCounter++;
        counterElapsed += gameTime.ElapsedGameTime;
        if (counterElapsed < TimeSpan.FromSeconds(1)) return;
#if DEBUG
        Window.Title = $"{title} {fpsCounter} fps - {GC.GetTotalMemory(false) / 1048576f} MB";
#endif
        FPS = fpsCounter;
        fpsCounter = 0;
        counterElapsed -= TimeSpan.FromSeconds(1);

    }

    public void ExitGame() 
    {
        Exit();
    }
}

