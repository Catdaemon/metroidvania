using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Common;
using System.Collections.Generic;
using Penumbra;
using System.Linq;

class WorldEntity : IGameEntity
{
    public Vector2 Position { get; set; }
    public Size2 Size { get; set; }
    private TiledMap _map;
    private TiledMapRenderer _mapRenderer;

    private List<Body> tileBodies = new List<Body>();
    private List<Hull> tileHulls = new List<Hull>();

    public WorldEntity(ContentManager contentManager, GraphicsDevice graphicsDevice, string mapName) {
        _map = contentManager.Load<TiledMap>("maps/" + mapName);
        _mapRenderer = new TiledMapRenderer(graphicsDevice, _map);

        PhysicsController.World.Gravity = new Vector2(0, 2f);

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
                            _Vertices.Add(ConvertUnits.ToSimUnits(new Vector2(point.X, point.Y)));
                        }

                        var shape = PhysicsController.World.CreatePolygon(_Vertices, 1, ConvertUnits.ToSimUnits(obj.Position), 0, BodyType.Static);

                        shape.Tag = this;

                        shape.SetFriction(1);
                        shape.SetRestitution(0.3f);
                        tileBodies.Add(shape);
                    }
                }
            } else if (objectLayer.Name == "shadows") {
                foreach(var obj in objectLayer.Objects)
                {
                    var polys = obj as TiledMapPolygonObject;
                    if (polys != null) {
                        var pointsAsVectors = new List<Vector2>();
                        foreach (var point in polys.Points)
                        {
                            pointsAsVectors.Add(point);
                        }

                        var shadowHull = new Hull(pointsAsVectors) {
                            Position = obj.Position
                        };
                        LightingController.AddHull(shadowHull);
                        this.tileHulls.Add(shadowHull);
                    }
                }
            } else {
                foreach(var obj in objectLayer.Objects)
                {
                    var tileObject = obj as TiledMapObject;
                    if (tileObject == null) continue;
                    
                    var created = EntityManager.Create(tileObject.Type, tileObject.Position, tileObject.Properties);
                }
            }
        }
    }

    public void Update(float delta) {}
    public void Draw(float delta, SpriteBatch spriteBatch) {}

    public void DrawWorld(GameTime gameTime, OrthographicCamera camera) {
        foreach (var layer in _map.TileLayers)
        {
            if (layer.Name == "foreground") continue;
            _mapRenderer.Draw(layer, camera.GetViewMatrix(), null, null, 0f);
        }
    }

    public void DrawForeground(GameTime gameTime, OrthographicCamera camera) {
        
        foreach (var layer in _map.TileLayers)
        {
            
            if (layer.Name != "foreground") continue;
            _mapRenderer.Draw(layer, camera.GetViewMatrix(), null, null, 0f);
        }
    }

    public void UpdateWorld(GameTime gameTime) {
        _mapRenderer.Update(gameTime);
    }

    public void Remove() {
        this.tileHulls.ForEach(x => LightingController.RemoveHull(x));
        this.tileBodies.ForEach(x => PhysicsController.World.Remove(x));
    }
}