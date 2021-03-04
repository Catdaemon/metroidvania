using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

public static class EntityManager
{
    public static List<IGameEntity> AllEntities = new List<IGameEntity>();

    public static IGameEntity Create(string TypeName, Vector2 Position)
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