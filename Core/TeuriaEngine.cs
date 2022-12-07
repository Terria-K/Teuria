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

    public static string ContentPath;
    
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
    private SubViewport subViewport;
    private bool resizing;
    private static bool fullscreen;
    private static int padding = 0;
    public static int Padding 
    {
        get => padding;
        set 
        {
            padding = value;
            Instance.UpdateView();
        }
    }

    public static int ScreenHeight { get; private set; }
    public static int ScreenWidth { get; private set; }
    public static int ViewWidth { get; private set; }
    public static int ViewHeight { get; private set; }
    public static bool Fullscreen 
    { 
        get => fullscreen;
        set 
        {
            fullscreen = value;
            Instance.graphics.HardwareModeSwitch = !fullscreen;
        }
    }

    public static int InternalID { get; internal set; }

    public TeuriaEngine(int width, int height, int screenWidth, int screenHeight, string windowTitle, bool fullScreen)
    {
#if DEBUG
#if SYSTEMTEXTJSON
        Console.WriteLine("Using SYSTEMTEXTJSON");
#else
        Console.WriteLine("Using Newtonsoft.JSON");
#endif
#endif
        instance = this;
        Window.Title = windowTitle;
        title = windowTitle;
        ScreenWidth = screenWidth;
        ScreenHeight = screenHeight;
        ViewWidth = width;
        ViewHeight = height;
        Window.ClientSizeChanged += OnClientSizeChanged;
        graphics = new GraphicsDeviceManager(this);
        graphics.HardwareModeSwitch = !fullScreen;
        graphics.IsFullScreen = fullScreen;
        ContentPath = "Content";


        Content.RootDirectory = ContentPath;
        Window.AllowUserResizing = true;
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
            subViewport.ScreenResolution = new Point(
                graphics.PreferredBackBufferWidth, 
                graphics.PreferredBackBufferHeight);
            UpdateView();
            resizing = false;
        }
    }

    private void UpdateView() 
    {
        var screen = new Vector2(
            GraphicsDevice.PresentationParameters.BackBufferWidth, 
            GraphicsDevice.PresentationParameters.BackBufferHeight
        );

        ViewSize(ref screen);
        var aspect = ViewHeight / (float)ViewWidth;
        ViewWidth -= Padding * 2;
        ViewHeight -= (int)(aspect * Padding * 2);

        ScreenMatrix = Matrix.CreateScale(
            ViewWidth / (float)ScreenWidth);

        viewport = new Viewport() 
        {
            X = (int)(screen.X / 2 - ViewWidth / 2),
            Y = (int)(screen.Y / 2 - ViewHeight / 2),
            Width = ViewWidth,
            Height = ViewHeight,
            MinDepth = 0,
            MaxDepth = 1
        };

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

    protected override sealed void Initialize()
    {
        graphics.PreferredBackBufferWidth = ScreenWidth;
        graphics.PreferredBackBufferHeight = ScreenHeight;
        graphics.ApplyChanges();
        Init();
        UpdateView();
        subViewport = new SubViewport(new Point(540, 320), GraphicsDevice, Color.Black);
        subViewport.SamplerState = SamplerState.PointClamp;
        subViewport.ScreenResolution = new Point(ScreenWidth, ScreenHeight);
        // ScreenMatrix = Matrix.CreateScale(ViewWidth / (float)ScreenHeight);
        scene.Initialize();

        base.Initialize();
    }

    protected override sealed void LoadContent()
    {
        TInput.Initialize();
        spriteBatch = new SpriteBatch(GraphicsDevice);
        Canvas.Initialize(graphics.GraphicsDevice, spriteBatch);
        Load();
        scene.Activate(spriteBatch);
        scene.Hierarchy(graphics.GraphicsDevice);
    }

    protected override sealed void UnloadContent()
    {
        Scene.Exit();
        CleanUp();
        Canvas.Dispose();
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
        RectangleShape.DebugRender = Keyboard.GetState().IsKeyDown(Keys.F1);
#endif
        
        if (scene != nextScene) 
        {
            scene.Exit();
            scene = nextScene;
            scene?.Activate(spriteBatch);
            scene?.Hierarchy(graphics.GraphicsDevice);
        }
        scene.ProcessLoop();


        base.Update(gameTime);
    }


    protected override void Draw(GameTime gameTime)
    {
        Scene?.BeforeRender();
        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Color.Black);
        GraphicsDevice.Viewport = viewport;
        // Scene?.Render();
        subViewport.Begin();
        Scene.Render();
        subViewport.End();

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

    protected virtual void Init() { throw new Exception(@"
    You don't have a game initialize yet! Make a game class first before you proceed."); }
    protected virtual void Load() {}
    protected virtual void Process(GameTime gameTime) {}
    protected virtual void CleanUp() {}
}

