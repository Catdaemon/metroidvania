using ImpromptuNinjas.UltralightSharp.Safe;
using ImpromptuNinjas.UltralightSharp.Enums;
using MonoGame.Extended;
using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public static class WebUIRenderer
    {
        public static ImpromptuNinjas.UltralightSharp.Safe.View WebView;
        public static ImpromptuNinjas.UltralightSharp.Safe.Renderer Renderer;
        public static ImpromptuNinjas.UltralightSharp.Safe.Session Session;
 
        public static void Setup(string Source, Size2 size)
        {
            if (WebView == null)
            {
                var asmDir = Path.GetDirectoryName(".");
                var tempDir = Path.GetTempPath();
                string storagePath;
                do
                    storagePath = Path.Combine(tempDir, Guid.NewGuid().ToString());
                while (Directory.Exists(storagePath) || File.Exists(storagePath));

                using (var cfg = new ImpromptuNinjas.UltralightSharp.Safe.Config())
                {
                    var cachePath = Path.Combine(storagePath, "Cache");
                    cfg.SetCachePath(cachePath);

                    var resourcePath = Path.Combine(asmDir, "resources");
                    cfg.SetResourcePath(resourcePath);

                    cfg.SetUseGpuRenderer(false);
                    cfg.SetEnableImages(true);
                    cfg.SetEnableJavaScript(false);

                    ImpromptuNinjas.UltralightSharp.Safe.AppCore.EnablePlatformFontLoader();

                    var assetsPath = Path.Combine(asmDir, "assets");
                    ImpromptuNinjas.UltralightSharp.Safe.AppCore.EnablePlatformFileSystem(assetsPath);

                    Renderer = new ImpromptuNinjas.UltralightSharp.Safe.Renderer(cfg);
                    Session = new ImpromptuNinjas.UltralightSharp.Safe.Session(Renderer, false, "game");
                    WebView = new ImpromptuNinjas.UltralightSharp.Safe.View(Renderer, (uint)size.Width, (uint)size.Height,true, Session);                    
                }
            } else {
                WebView.Resize((uint)size.Width, (uint)size.Height);
            }
            WebView.LoadHtml(@"
                <div style='width:100vw;height:4em;background-color:rgba(128,128,128,64);border-radius:4px;'>lel top</div>

                <div style='position:fixed;bottom:0;left:0;width:100vw;height:4em;background-color:rgba(128,128,128,64);border-radius:4px;'>lel bottom</div>
                
            ");
            //WebView.LoadUrl(Source);
        }

        public static void Update()
        {
            Console.WriteLine(WebView.IsLoading());
            Renderer.Update();
            Renderer.Render();
        }

        public static unsafe Texture2D GetTexture(GraphicsDevice graphicsDevice)
        {
            
            var surface = WebView.GetSurface();
            var bitmap = surface.GetBitmap();
            
            var pixels = bitmap.LockPixels();
            
            var pixelsPointer = (byte*)pixels;

            var format = bitmap.GetFormat();

            var width = (int)bitmap.GetWidth();
            var height = (int)bitmap.GetHeight();
            var stride = (int)bitmap.GetRowBytes();
            var bpp = (int)bitmap.GetBpp();

            int arraySize = (int)bitmap.GetWidth() * (int)bitmap.GetHeight() * bpp;

            using (var ms = new UnmanagedMemoryStream(pixelsPointer, arraySize))
            {
                byte[] buffer = new byte[arraySize];

                ms.Read(buffer, 0, arraySize);
                
                for (int i = 0; i < buffer.Length - 2; i += 4)
                {
                    byte r = buffer[i];
                    buffer[i] = buffer[i + 2];
                    buffer[i + 2] = r;
                }

                var tex = new Texture2D(graphicsDevice, (int)bitmap.GetWidth(), (int)bitmap.GetHeight());
                tex.SetData<byte>(buffer, 0, arraySize);

                bitmap.UnlockPixels();
                return tex;
            }
        }        
    }