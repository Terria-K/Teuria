using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Teuria;

public abstract class GameApp : Game
{
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private Scene scene;
    private Scene nextScene;
    private int fpsCounter;
    private TimeSpan counterElapsed = TimeSpan.Zero;
    private string title;
    public static Matrix ScreenMatrix;
    public static Viewport Viewport { get => viewport; private set => viewport = value; }
    private static Viewport viewport;

    public static string ContentPath;
    public GraphicsDeviceManager GraphicsDeviceManager => graphics;
    
    public static GameApp Instance => instance;
    public Scene Scene 
    { 
        get => scene; 
        set 
        { 
            scene ??= value;
            nextScene = value; 
        } 
    }
    private static GameApp instance;
    

    public static int ScreenHeight { get; private set; }
    public static int ScreenWidth { get; private set; }
    public static int ViewWidth { get; private set; }
    public static int ViewHeight { get; private set; }

    public static int InternalID { get; internal set; }
#region NewFeatures
    // public static ScreenView ScreenView;
    private RenderTarget2D teuriaBackBuffer;
    private Rectangle windowRect;
    private Rectangle boxingRect;
    public float WindowAspect => Window.ClientBounds.Width / (float)Window.ClientBounds.Height;

    public Rectangle Screen => boxingRect;
    public Rectangle WindowScreen => windowRect;
    private float aspect;
#endregion

#if ANDROID
    public GameApp(int width, int height, string windowTitle)
#else
    public GameApp(int width, int height, int screenWidth, int screenHeight, string windowTitle, bool fullScreen)
#endif
    {
        instance = this;
        Window.Title = windowTitle;
        title = windowTitle;
#if !ANDROID
        ScreenWidth = screenWidth;
        ScreenHeight = screenHeight;
#endif
        ViewWidth = width;
        ViewHeight = height;

        graphics = new GraphicsDeviceManager(this);
#if !ANDROID
#if !FNA
        graphics.HardwareModeSwitch = !fullScreen;
#endif //FNA
        graphics.IsFullScreen = fullScreen;
        graphics.PreferredBackBufferWidth = ScreenWidth;
        graphics.PreferredBackBufferHeight = ScreenHeight;
#endif //ANDROID
        ContentPath = "Content";
        Content.RootDirectory = ContentPath;
        Window.AllowUserResizing = true;
        IsMouseVisible = true;
    }

    private void OnClientSizeChanged(object sender, EventArgs e) 
    {
        int windowWidth, windowHeight;

        if (graphics.IsFullScreen) 
        {
            windowWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            windowHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        }
        else 
        {
            windowWidth = Window.ClientBounds.Width;
            windowHeight = Window.ClientBounds.Height;
        }

        if (WindowAspect <= aspect) 
        {
            int presentHeight = (int)((windowWidth / aspect) + 0.5f);
            int barHeight = (windowHeight - presentHeight) / 2;
            boxingRect = new Rectangle(0, barHeight, windowWidth, presentHeight);
        }
        else 
        {
            int presentWidth = (int)((windowHeight * aspect) + 0.5f);
            int barWidth = (windowWidth - presentWidth) / 2;
            boxingRect = new Rectangle(barWidth, 0, presentWidth, windowHeight);
        }
        GraphicsDevice.Viewport = new Viewport() 
        {
            X = boxingRect.X,
            Y = boxingRect.Y,
            Width = boxingRect.Width,
            Height = boxingRect.Height,
            MinDepth = 0,
            MaxDepth = 1
        };
        GraphicsReset();
    }
    
    public void ToggleFullscreen() 
    {
        graphics.IsFullScreen = !graphics.IsFullScreen;
#if !FNA
        graphics.HardwareModeSwitch = !graphics.IsFullScreen;
#endif
        if (graphics.IsFullScreen) 
        {
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        }
        else 
        {
            graphics.PreferredBackBufferWidth = windowRect.Width;
        }
        graphics.ApplyChanges();
    }

    protected override sealed void Initialize()
    {
        if (!graphics.IsFullScreen)
            windowRect = new Rectangle(0, 0, ScreenWidth, ScreenHeight);
        else    
            windowRect = new Rectangle(
                0, 0, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
        boxingRect = windowRect;
        aspect = windowRect.Width / (float)windowRect.Height;
        graphics.PreferredBackBufferWidth = windowRect.Width;
        graphics.PreferredBackBufferHeight = windowRect.Height;
        graphics.ApplyChanges();

        teuriaBackBuffer = new RenderTarget2D(GraphicsDevice, ViewWidth, ViewHeight);

        Window.ClientSizeChanged += OnClientSizeChanged;
        Init();

        base.Initialize();
    }

    protected override sealed void LoadContent()
    {
        TInput.Initialize();
        spriteBatch = new SpriteBatch(GraphicsDevice);
        Canvas.Initialize(graphics.GraphicsDevice, spriteBatch);
        Load();
        scene?.Activate(spriteBatch);
        scene?.Hierarchy(graphics.GraphicsDevice);
    }

    protected override sealed void UnloadContent()
    {
        SkyLog.Log("Game Shutdown", SkyLog.LogLevel.Info);
        TInput.Shutdown();
        Scene.Exit();
        CleanUp();
        Canvas.Dispose();
        foreach(var textures in TeuriaImporter.cleanupCache) 
        {
            textures?.Dispose();
        }
        base.UnloadContent();
    }

    protected override sealed void Update(GameTime gameTime)
    {   
        Time.Delta = (float)gameTime.ElapsedGameTime.TotalSeconds * Time.DeltaScale;
        TInput.Update();
        Process(gameTime);

#if DEBUG
        Shape.DebugRender = Keyboard.GetState().IsKeyDown(Keys.F1);
#endif
        
        if (scene != nextScene) 
        {
            scene.Exit();
            scene = nextScene;
            scene?.Activate(spriteBatch);
            scene?.Hierarchy(graphics.GraphicsDevice);
        }


        scene.Process();


        base.Update(gameTime);
    }


    protected override sealed void Draw(GameTime gameTime)
    {
        Draw();
        Scene?.BeforeRender();

        GraphicsDevice.SetRenderTarget(teuriaBackBuffer);
        GraphicsDevice.Clear(Color.Black);

        Scene?.Render();

        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Color.Black);
        spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
        spriteBatch.Draw(teuriaBackBuffer, boxingRect, Color.White);
        spriteBatch.End();
        Scene?.AfterRender();



        base.Draw(gameTime);

        fpsCounter++;
        counterElapsed += gameTime.ElapsedGameTime;
        if (counterElapsed < TimeSpan.FromSeconds(1)) return;
#if DEBUG
        Window.Title = $"{title} {fpsCounter} fps - {GC.GetTotalMemory(false) / 1048576f} MB";
#endif
        Time.FPS = fpsCounter;
        fpsCounter = 0;
        counterElapsed -= TimeSpan.FromSeconds(1);
    }


    public void ExitGame() 
    {
        Exit();
    }

    protected abstract void Init();
    protected virtual void Load() {}
    protected virtual void Draw() {}
    protected virtual void GraphicsReset() {}
    protected virtual void Process(GameTime gameTime) {}
    protected virtual void CleanUp() {}
}

