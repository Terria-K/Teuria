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
    public static Matrix ScreenMatrix;
    public static Viewport Viewport { get => viewport; private set => viewport = value; }
    private static Viewport viewport;
    
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
    private Resolution res;
    private bool resizing;
    private static int padding = 0;
    public static int Padding 
    {
        get => padding;
        set 
        {
            padding = value;
            // Instance.UpdateView();
        }
    }

    public static int ScreenHeight { get; private set; }
    public static int ScreenWidth { get; private set; }
    public static int ViewWidth { get; private set; }
    public static int ViewHeight { get; private set; }
    public static bool Fullscreen { get; set; }

    public TeuriaEngine(int width, int height, int screenWidth, int screenHeight, string windowTitle, bool fullScreen)
    {
        instance = this;
        Window.Title = windowTitle;
        title = windowTitle;
        ViewHeight = screenHeight;
        ViewWidth = screenWidth;
        Fullscreen = fullScreen;
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += OnClientSizeChanged;
        graphics = new GraphicsDeviceManager(this);
        graphics.HardwareModeSwitch = false;
        graphics.IsFullScreen = fullScreen;

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    private void OnClientSizeChanged(object sender, EventArgs e) 
    {
        if ((GraphicsDevice.Viewport.Width != graphics.PreferredBackBufferWidth || 
            GraphicsDevice.Viewport.Height != graphics.PreferredBackBufferHeight) && !resizing) 
        {
            resizing = true;
            if (Window.ClientBounds.Width == 0)  
            {
                resizing = false;
                return;
            }
            graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            graphics.ApplyChanges();
            res.ScreenResolution = new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            UpdateView();
            resizing = false;
        }
    }

    private void UpdateView() 
    {
        var screen = new Vector2(
            GraphicsDevice.PresentationParameters.BackBufferWidth, 
            GraphicsDevice.PresentationParameters.BackBufferHeight);

        ViewSize(ref screen);
        var aspect = ViewHeight / (float)ViewWidth;
        ViewWidth -= Padding * 2;
        ViewHeight -= (int)(aspect * Padding * 2);

        ScreenMatrix = Matrix.CreateScale(ViewWidth / (float)ScreenWidth);

        Viewport = new Viewport() 
        {
            X = (int)(screen.X / 2 - ViewWidth / 2),
            Y = (int)(screen.Y / 2 - ViewHeight / 2),
            Width = ViewWidth,
            Height = ViewHeight,
            MinDepth = 0,
            MaxDepth = 1
        };
        res.UpdateResolution(ref viewport);

    }

    private void ViewSize(ref Vector2 screen) 
    {
        if (screen.X / ScreenWidth > screen.Y / ScreenHeight) 
        {
            ViewWidth = (int)(screen.Y / ScreenHeight * ScreenWidth);
            ViewHeight = (int)screen.Y;
            return;
        }
        ViewWidth = (int)screen.X;
        ViewHeight = (int)(screen.X / ScreenWidth * ScreenHeight);
    }

    protected virtual SceneRenderer Init() { throw new NotImplementedException(); }
    protected virtual void Load() {}
    protected virtual void Process(GameTime gameTime) {}
    protected virtual void CleanUp() {}

    protected override sealed void Initialize()
    {
        graphics.PreferredBackBufferWidth = ViewWidth;
        graphics.PreferredBackBufferHeight = ViewHeight;
        graphics.ApplyChanges();
        ScreenHeight = graphics.PreferredBackBufferHeight;
        ScreenWidth = graphics.PreferredBackBufferWidth;
        renderer = Init();
        res = new Resolution(new Point(ViewWidth, ViewHeight), GraphicsDevice, renderer.EnvironmentColor);
        res.ScreenResolution = new Point(ViewWidth, ViewHeight);
        UpdateView();
        // ScreenMatrix = Matrix.CreateScale(ViewWidth / (float)ScreenHeight);
        scene.Initialize();

        base.Initialize();
    }

    protected override sealed void LoadContent()
    {
        Canvas.Initialize(graphics.GraphicsDevice);
        TInput.Initialize();
        spriteBatch = new SpriteBatch(GraphicsDevice);
        Load();
        
        scene.Ready(graphics.GraphicsDevice);
    }

    protected override sealed void UnloadContent()
    {
        Scene.Exit();
        CleanUp();
        foreach(var textures in TextureImporter.cleanupCache) 
        {
            textures?.Dispose();
        }
        base.UnloadContent();
    }

    protected override sealed void Update(GameTime gameTime)
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
        GraphicsDevice.Viewport = Viewport;
        res.Begin();
        renderer.Draw(spriteBatch);
        res.End();

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

