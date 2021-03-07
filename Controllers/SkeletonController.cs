using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using MonoGame.Extended;
using Spine;

public static class SkeletonController
{
    private static GraphicsDevice _graphics;
    public static SkeletonRenderer renderer;
    public static Dictionary<string, Atlas> atlasCache = new Dictionary<string, Atlas>();
    public static Dictionary<string, SkeletonData> skeletonDataCache = new Dictionary<string, SkeletonData>();

    public static void Init(GraphicsDevice graphicsDevice)
    {
        _graphics = graphicsDevice;
        renderer = new SkeletonRenderer(graphicsDevice);
        //renderer.PremultipliedAlpha = false;
    }

    public static Atlas GetAtlas(string path)
    {
        if (atlasCache.ContainsKey(path)) return atlasCache[path];

        var atlas = new Atlas(path, new XnaTextureLoader(_graphics));
        atlasCache.Add(path, atlas);

        return atlas;
    }

    public static Skeleton GetSkeleton(string path, Atlas atlas)
    {
        SkeletonData data;

        if (skeletonDataCache.ContainsKey(path))
        {
            data = skeletonDataCache[path];
        } else {
            var skeletonBinary = new SkeletonBinary(atlas);
            data = skeletonBinary.ReadSkeletonData(path);
        }

        return new Skeleton(data);
    }

    public static void LoadContent()
    {
    }

    public static void UpdateCameraMatrix(OrthographicCamera camera)
    {
        var camProj = Matrix.CreateOrthographic(_graphics.Viewport.Width * camera.Zoom, _graphics.Viewport.Height * camera.Zoom, 0f, 30f);
        camProj = Matrix.CreateOrthographicOffCenter(0, _graphics.Viewport.Width, _graphics.Viewport.Height, 0, 1, 0);
        var camView = camera.GetViewMatrix();
        ((BasicEffect)renderer.Effect).Projection = camProj;
        ((BasicEffect)renderer.Effect).View = camView;
    }

    public static void Update(Matrix transform)
    {
    }
    public static void Draw(GameTime gameTime)
    {
    }
}