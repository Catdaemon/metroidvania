using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Collision;
using tainicom.Aether.Physics2D.Dynamics.Contacts;
using tainicom.Aether.Physics2D.Common;
using System;
using System.Collections.Generic;

public class CollisionResult
{
    public bool Hit = false;
    public IGameEntity Entity = null;
    public bool IsWorld = false;
    public Vector2 Normal;
    public Vector2 Point;
}

public static class PhysicsController
{
    public static World World = new World();
    public static float WorldScale = 10f;

    public static CollisionResult RayCastFirst(Vector2 point_0, Vector2 point_1, IGameEntity ignore)
    {
        var _asList = new List<IGameEntity>();
        _asList.Add(ignore);

        return RayCastFirst(point_0, point_1, _asList);
    }

    public static CollisionResult RayCastFirst(Vector2 point_0, Vector2 point_1, IEnumerable<IGameEntity> ignore = null)
    {
        CollisionResult result = new CollisionResult();
        
        RayCastReportFixtureDelegate get_first_callback = delegate(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
        {
            if (fixture.Body.Tag is IGameEntity) {
                var hitEnt = (IGameEntity)fixture.Body.Tag;
                if (ignore != null) {
                    foreach(var ignored in ignore) {
                        if (ignored == hitEnt) {
                            return -1;
                        }
                    }
                }

                result.Entity = hitEnt;
            }

            if (fixture.Body.Tag is WorldEntity) {
                result.Entity = (IGameEntity)fixture.Body.Tag;
                result.IsWorld = true;
            }
            result.Normal = normal;
            result.Point = point;
            result.Hit = true;

            return fraction;
        };

        // Summary:
        //     Ray-cast the world for all fixtures in the path of the ray. Your callback
        //     controls whether you get the closest point, any point, or n-points.  The
        //     ray-cast ignores shapes that contain the starting point.  Inside the callback:
        //     return -1: ignore this fixture and continue 
        //     return  0: terminate the ray cast
        //     return fraction: clip the ray to this point
        //     return 1:        don't clip the ray and continue
        World.RayCast(get_first_callback, ConvertUnits.ToSimUnits(point_0), ConvertUnits.ToSimUnits(point_1));

        return result;
    }

}