# Teuria
 A Simple Game Engine made from MonoGame
 
 This game engine includes a basic functionality you need for 2D game development such as ECS.
 
 This engine is still work-in-progress and still needs more improvements. It is heavily inspired from Monocle and Godot Engine. It is not usable for larger projects, but it is useful for starters who wants to get in to Monogame.

 Monocle: https://github.com/JamesMcMahon/monocle-engine

 Godot Engine: https://godotengine.org/
 
 - [x] ECS
 - [x] Scenes
 - [x] Coroutines
 - [x] Draw Images
 - [x] Camera
 - [x] Physics
 - [ ] Audio
 - [ ] User Interfaces
 

### Getting Started

We first need to create a game class that is inherits to Engine, and instance the game in `Program.cs`
```csharp
using Teuria;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
// YourGame.cs
public class YourGame : TeuriaEngine
{
    public YourGame(int width, int height, int screenWidth, int screenHeight, string windowTitle, bool fullScreen) : base(width, height, screenWidth, screenHeight, windowTitle, fullScreen)
    {
    }

    /** We need to setup the scene renderer before going any further */
    protected override SceneRenderer Init()
    {
        var camera = new Camera();
		var mainScene = new MainScene(Content, camera);
        return new SceneRenderer(mainScene, camera);
    }
}

// MainScene.cs
public class MainScene : Scene 
{
	public MainScene(ContentManager content, Camera camera) : base(content, camera) {}
	
	public override void Ready(GraphicsDevice device) 
	{
		/** Load Content Here */
		// We need the font first, but we can use the built-in font
		var font = FontText.Create("Teuria/Rubik-Regular");
		
		/** Instance any Entity here */
		var helloWorld = new Label(font);
		
		/** Add them to the scene */
		Add(helloWorld);
		base.Ready(device);
	}
}

// Program.cs
public static class Program
{
    [STAThread]
    static void Main()
    {
        using (var game = new PongPing(720, 1024, 480, 720, "Pong Ping", false))
            game.Run();
    }
}
```

### Switching through Scene
Switching scene is also very simple, you just need to replace the current scene from the game using SceneRenderer.

```csharp
// We need to change the scene using the SceneRenderer built-in to Scene
var scene = new NewScene(Content, null);
SceneRenderer.ChangeScene(scene);

```

### Moving the Camera
Here is how you can move the camera, allowing you to make a cutscene, shake effects and more possibilities!

```csharp
// Change the position
Camera.Position = new Vector2(45f, 45f);
// Change Zoom
Camera.Zoom = new Vector2(1.2f, 1.2f);
// Change Offset
Camera.Offset = new Vector2(65f, 65f);
```

### Loading an image

There is two ways to load an image, Content ways or TextureImporter ways. The TextureImporter will just use the `Texture2D.FromStream()` to load an image from stream, but this is also managed by the engine and will be freed automatically when the program is terminated or freed by Sprite when cleanup is enabled.

```csharp
// Remember to always load image in 'Ready'.
public override void Ready(GraphicsDevice device) 
{
	// This is loaded using MGCB Editor
	var mgcbImg = Content.Load<Texture2D>("bat");
	// This is loaded using TextureImporter Load Image
	var texImpImg = TextureImporter.LoadImage(device, "bat.png");

	// Manually Dispose an image loaded
	mgcbImg.Dispose();
	TextureImporter.CleanUp(texImpImg);
	base.Ready(device);
}
```