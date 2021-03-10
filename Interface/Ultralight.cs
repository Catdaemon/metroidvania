using ImpromptuNinjas.UltralightSharp.Safe;
using ImpromptuNinjas.UltralightSharp.Enums;
using ImpromptuNinjas.UltralightSharp;
using MonoGame.Extended;
using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
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
                    cfg.SetEnableJavaScript(true);

                    ImpromptuNinjas.UltralightSharp.Safe.AppCore.EnablePlatformFontLoader();

                    var assetsPath = Path.Combine(asmDir, "assets");
                    ImpromptuNinjas.UltralightSharp.Safe.AppCore.EnablePlatformFileSystem(assetsPath);

                    Renderer = new ImpromptuNinjas.UltralightSharp.Safe.Renderer(cfg);
                    Session = new ImpromptuNinjas.UltralightSharp.Safe.Session(Renderer, false, "game");
                    WebView = new ImpromptuNinjas.UltralightSharp.Safe.View(Renderer, (uint)size.Width, (uint)size.Height, true, Session);                    
                }
            } else {
                WebView.Resize((uint)size.Width, (uint)size.Height);
            }

            var ctx = WebView.LockJsContext();
            
            
            unsafe {
                ImpromptuNinjas.UltralightSharp.JsValue* func1(ImpromptuNinjas.UltralightSharp.JsContext* ctx,
                    ImpromptuNinjas.UltralightSharp.JsValue* function,
                    ImpromptuNinjas.UltralightSharp.JsValue* thisObject,
                    UIntPtr argumentCount,
                    ImpromptuNinjas.UltralightSharp.JsValue** arguments,
                    ImpromptuNinjas.UltralightSharp.JsValue** exception
                )
                {
                    
                    Console.WriteLine("js called");

                    var localCtx = new JsLocalContext(ctx);
                    var argCount = (int)argumentCount;
                    var args = new JsValueLike?[argCount];
                    for (var i = 0; i < argCount; ++i)
                    {
                        var arg = JsValueLike.Create(arguments[i], localCtx);
                        var t = arg.GetJsType();
                        if (t == JsType.String)
                        {
                            JsValueLike exc;
                            var s = arg.ToStringCopy(out exc);
                            string str = s;
                            Console.WriteLine(str);
                        }

                    }

                    //ImpromptuNinjas.UltralightSharp.Safe.JsString retval = "";
                    return null;// (ImpromptuNinjas.UltralightSharp.JsValue*)retval.Unsafe; crash?
                }
                

                var p = new FnPtr<ImpromptuNinjas.UltralightSharp.ObjectCallAsFunctionCallback>(func1);

                ImpromptuNinjas.UltralightSharp.Safe.JsString funcName = "test";

                
                var funcRef = ImpromptuNinjas.UltralightSharp.JavaScriptCore.JsObjectMakeFunctionWithCallback(ctx.Unsafe, funcName.Unsafe, p);
                var oref = ctx.GetGlobalObject();

                ImpromptuNinjas.UltralightSharp.JavaScriptCore.JsObjectSetProperty(ctx.Unsafe, oref.Unsafe, funcName.Unsafe, funcRef, JsPropertyAttribute.ReadOnly, null);
            }
            
            WebView.UnlockJsContext();

            var testhtml = File.ReadAllText("test/testui.html");

            WebView.LoadHtml(testhtml);
        }

        
        public static void SetData(object data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            //var js = WebView.LockJsContext();            
            WebView.EvaluateScript($"receiveData({jsonString})");
            //WebView.UnlockJsContext();
        }

        public static void Update()
        {
            var mousePos = Mouse.GetState().Position;

            SetMousePos(mousePos);
            if (InputController.LeftClickPressed)
            {
                InjectMouseDown(mousePos);
            } 
            if (InputController.LeftClickReleased) {
                InjectMouseUp(mousePos);
            }

            Renderer.Update();
        }

        public static void InjectMouseDown(Point pos)
        {
            WebView.FireMouseEvent(new ImpromptuNinjas.UltralightSharp.Safe.MouseEvent(MouseEventType.MouseDown, (int)pos.X, (int)pos.Y, MouseButton.Left));
        }
        public static void InjectMouseUp(Point pos)
        {
            WebView.FireMouseEvent(new ImpromptuNinjas.UltralightSharp.Safe.MouseEvent(MouseEventType.MouseUp, (int)pos.X, (int)pos.Y, MouseButton.Left));
        }

        public static void InjectKeyboard()
        {
            //WebView.FireKeyEvent(new KeyEvent(KeyEventType.))
        }

        public static void SetMousePos(Point pos)
        {
            WebView.FireMouseEvent(new ImpromptuNinjas.UltralightSharp.Safe.MouseEvent(MouseEventType.MouseMoved, (int)pos.X, (int)pos.Y, MouseButton.None));
        }

        public static unsafe Texture2D GetTexture(GraphicsDevice graphicsDevice)
        {
            Renderer.Render();
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