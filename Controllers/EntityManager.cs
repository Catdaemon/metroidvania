using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.Tiled;
public static class EntityManager
{
    public static List<IGameEntity> AllEntities = new List<IGameEntity>();

    public static IGameEntity Create(string TypeName, Vector2 Position, TiledMapProperties Props)
    {
        IGameEntity created = null;

        switch (TypeName)
        {
            case "ball":
                created = new Ball();
                break;
            case "rat":
                created = new Rat();
                break;
            case "bat":
                // do nothing
                break;
            case "light":
                var color = Props.Where(x => x.Key == "color").FirstOrDefault().Value;
                var radius = Props.Where(x => x.Key == "radius").FirstOrDefault().Value;
                
                var _color = new Color(0,0,0);
                if (!String.IsNullOrEmpty(color))
                {
                    var fromhex = System.Drawing.ColorTranslator.FromHtml(color);
                    _color = new Color(fromhex.R, fromhex.G, fromhex.B);
                }

                var _radius = 3000f;
                if (!String.IsNullOrEmpty(radius))
                {
                    float.TryParse(radius, System.Globalization.NumberStyles.Any, null, out _radius);
                }

                created = new WorldLight(Position, _radius, _color);
                
                break;
            default:
                throw new NotImplementedException();
        }

        if (created != null) {
            created.Position = Position;
            AllEntities.Add(created);
        }        

        return created;
    }

    public static void Remove(IGameEntity toRemove)
    {
        AllEntities.Remove(toRemove);
    }

    public static IEnumerable<IGameEntity> GetPlayers()
    {
        return AllEntities.Where(x => x is Player);
    }

    public static IGameEntity GetClosestPlayer(Vector2 position)
    {
        return GetPlayers().OrderBy(x => (x.Position - position).Length()).FirstOrDefault();
    }
}