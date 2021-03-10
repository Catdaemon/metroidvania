using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using tainicom.Aether.Physics2D.Diagnostics;
using Penumbra;

namespace metroidvania
{
    public class GameMain : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private WorldEntity world;
        private Player LocalPlayer;

        
        private DebugView DebugView = null;
        private FramesPerSecondCounter FPSCounter;

        public bool isPaused = false;
        private float GameSpeed = 1;


        public GameMain()
        {            
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8
            };
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            LightingController.Init(this);
            Components.Add(LightingController.Penumbra);

            SkeletonController.Init(GraphicsDevice);
        }

        protected override void Initialize()
        {
            CameraController.Setup(GraphicsDevice);
            InputController.Setup(CameraController.Camera);
            WebUIRenderer.Setup("https://news.bbc.co.uk", new Size2(1920, 1080));

            FPSCounter = new FramesPerSecondCounter();

            world = new WorldEntity(Content, GraphicsDevice, "untitled");

            LocalPlayer = new Player();
            LocalPlayer.Position = new Vector2(200, 200);
            EntityManager.AllEntities.Add(LocalPlayer);

            DebugView = new DebugView(PhysicsController.World);
            DebugView.DefaultShapeColor = Color.White;
            DebugView.SleepingShapeColor = Color.LightGray;
            DebugView.LoadContent(GraphicsDevice, Content);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            LightingController.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (InputController.PausePressed) {
                isPaused = !isPaused;
            }
            if (InputController.PauseReleased) {} // for now, bad implementation needs this call

            if (isPaused) {
                GameSpeed = MathHelper.Lerp(GameSpeed, 0, delta * 4);
            } else {
                GameSpeed = MathHelper.Lerp(GameSpeed, 1, delta * 4);
            }
            

            world.UpdateWorld(gameTime);

            foreach (var ent in EntityManager.AllEntities) {
                ent.Update(delta * GameSpeed);
            }

            var kb = Keyboard.GetState();

            CameraController.Update(delta * GameSpeed, LocalPlayer.Position, LocalPlayer.Body.LinearVelocity.NormalizedCopy());

            PhysicsController.World.Step((float)gameTime.ElapsedGameTime.TotalSeconds * GameSpeed);

            FPSCounter.Update(gameTime);

            WebUIRenderer.SetData(new GameData() {
                isMainMenu = false,
                isPaused = isPaused,
                player = new {
                    LocalPlayer.Position.X,
                    LocalPlayer.Position.Y
                }
            });
            WebUIRenderer.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var transformMatrix = CameraController.Camera.GetViewMatrix();

            SkeletonController.UpdateCameraMatrix(CameraController.Camera);

            LightingController.BeginDraw(transformMatrix);

            GraphicsDevice.Clear(Color.Black);


            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
            world.DrawWorld(gameTime, CameraController.Camera);
            _spriteBatch.End();
            

            _spriteBatch.Begin(SpriteSortMode.Immediate, transformMatrix: transformMatrix);

            foreach (var ent in EntityManager.AllEntities) {
                ent.Draw(delta, _spriteBatch);
            }
            
            _spriteBatch.End();

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
            world.DrawForeground(gameTime, CameraController.Camera);
            _spriteBatch.End();

            LightingController.EndDraw(gameTime);         

            var camProj = Matrix.CreateOrthographic(GraphicsDevice.Viewport.Width * CameraController.Camera.Zoom, GraphicsDevice.Viewport.Height * CameraController.Camera.Zoom, 0f, 30f);
            camProj = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 1, 0);
            var camView = CameraController.Camera.GetViewMatrix();


            DebugView.RenderDebugData(camProj, camView);

            FPSCounter.Draw(gameTime);

            
            var _tex = WebUIRenderer.GetTexture(GraphicsDevice);
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            _spriteBatch.Draw(_tex, Vector2.Zero, Color.White);
            _spriteBatch.End();




            //base.Draw(gameTime);
        }
    }
}
