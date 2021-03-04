using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System.Collections.Generic;
using tainicom.Aether.Physics2D.Diagnostics;

namespace metroidvania
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private OrthographicCamera _camera;

        private WorldEntity world;
        private Player LocalPlayer;

        
        private DebugView DebugView = null;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _camera = new OrthographicCamera(GraphicsDevice);

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
            
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            double delta = gameTime.ElapsedGameTime.TotalSeconds;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            
            world.UpdateWorld(gameTime);

            foreach (var ent in EntityManager.AllEntities) {
                ent.Update(delta);
            }

            var kb = Keyboard.GetState();
            var camSpeed = 50f;
            if (kb.IsKeyDown(Keys.Right)) _camera.Move(new Vector2(camSpeed * (float)delta,0));
            if (kb.IsKeyDown(Keys.Left)) _camera.Move(new Vector2(-camSpeed * (float)delta,0));
            if (kb.IsKeyDown(Keys.Up)) _camera.Move(new Vector2(0,-camSpeed * (float)delta));
            if (kb.IsKeyDown(Keys.Down)) _camera.Move(new Vector2(0,camSpeed * (float)delta));

            _camera.LookAt(LocalPlayer.Position);
            _camera.Zoom = 4f;

            PhysicsController.World.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            double delta = gameTime.ElapsedGameTime.TotalSeconds;
            
            GraphicsDevice.Clear(Color.CornflowerBlue);

            world.DrawWorld(gameTime, _camera);

            var transformMatrix = _camera.GetViewMatrix();
            _spriteBatch.Begin(transformMatrix: transformMatrix);

            foreach (var ent in EntityManager.AllEntities) {
                ent.Draw(delta, _spriteBatch);
            }
            
            _spriteBatch.End();

            var camProj = Matrix.CreateOrthographic(GraphicsDevice.Viewport.Width * _camera.Zoom, GraphicsDevice.Viewport.Height * _camera.Zoom, 0f, 30f);
            var cameraPosition = new Vector3(_camera.Center, 0f);
            var cameraUp = Vector3.TransformNormal(Vector3.Down, Matrix.CreateRotationZ(MathHelper.ToRadians(_camera.Rotation)));
            var camView = Matrix.CreateLookAt(cameraPosition, cameraPosition + Vector3.Backward, cameraUp);

            //DebugView.RenderDebugData(camProj, camView);


            base.Draw(gameTime);
        }
    }
}
