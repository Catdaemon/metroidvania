using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Common;
using System.Collections.Generic;

class WorldEntity : IGameEntity
{
    public Vector2 Position { get; set; }
    public Size2 Size { get; set; }
    private TiledMap _map;
    private TiledMapRenderer _mapRenderer;

    private List<Body> tileBodies = new List<Body>();

    public WorldEntity(ContentManager contentManager, GraphicsDevice graphicsDevice, string mapName) {
        _map = contentManager.Load<TiledMap>("maps/" + mapName);
        _mapRenderer = new TiledMapRenderer(graphicsDevice, _map);

        PhysicsController.World.Gravity = new Vector2(0, 200f);

        foreach(var objectLayer in _map.ObjectLayers)
        {
            if (objectLayer.Name == "collision")
            {
                foreach(var obj in objectLayer.Objects)
                {
                    var polys = obj as TiledMapPolygonObject;
                    if (polys != null) {
                        var _Vertices = new Vertices();
                        foreach (var point in polys.Points)
                        {
                            _Vertices.Add(new Vector2(point.X, point.Y));
                        }

                        var shape = PhysicsController.World.CreatePolygon(_Vertices, 1, obj.Position, 0, BodyType.Static);

                        shape.Tag = this;

                        shape.SetFriction(1);
                        shape.SetRestitution(0.3f);
                        tileBodies.Add(shape);
                    }
                }
            } else {
                foreach(var obj in objectLayer.Objects)
                {
                    var tileObject = obj as TiledMapObject;
                    if (tileObject == null) continue;
                    
                    var created = EntityManager.Create(tileObject.Type, tileObject.Position);
                }
            }
        }
    }

    public void Update(double delta) {}
    public void Draw(double delta, SpriteBatch spriteBatch) {}

    public void DrawWorld(GameTime gameTime, OrthographicCamera camera) {
        _mapRenderer.Draw(camera.GetViewMatrix());
    }

    public void UpdateWorld(GameTime gameTime) {
        _mapRenderer.Update(gameTime);
    }
}