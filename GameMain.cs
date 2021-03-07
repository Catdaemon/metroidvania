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


        public GameMain()
        {            
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8
            };
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
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
            WebUIRenderer.Setup("https://google.com", new Size2(1280, 720));

            FPSCounter = new FramesPerSecondCounter();

            world = new WorldEntity(Content, GraphicsDevice, "untitled");

            LocalPlayer = new Player();
            LocalPlayer.Position = new Vector2(800, 200);
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

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            world.UpdateWorld(gameTime);

            foreach (var ent in EntityManager.AllEntities) {
                ent.Update(delta);
            }

            var kb = Keyboard.GetState();

            CameraController.Update(delta, LocalPlayer.Position, LocalPlayer.Body.LinearVelocity.NormalizedCopy());

            PhysicsController.World.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

            FPSCounter.Update(gameTime);

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
