using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Penumbra;

public static class LightingController
{
    public static PenumbraComponent Penumbra;

    public static void Init(Game game)
    {
        Penumbra = new PenumbraComponent(game) {
            AmbientColor = new Color(new Vector3(0.6f))
        };
    }

    public static void LoadContent()
    {
        Penumbra.Initialize();
    }

    public static void AddLight(Light toAdd)
    {
        Penumbra.Lights.Add(toAdd);
    }
    public static void RemoveLight(Light toRemove)
    {
        Penumbra.Lights.Remove(toRemove);
    }

    public static void AddHull(Hull toAdd)
    {
        Penumbra.Hulls.Add(toAdd);
    }
    public static void RemoveHull(Hull toRemove)
    {
        Penumbra.Hulls.Remove(toRemove);
    }

    public static void BeginDraw(Matrix transform)
    {
        Penumbra.Transform = transform;
        Penumbra.BeginDraw();
    }
    public static void EndDraw(GameTime gameTime)
    {
        Penumbra.Draw(gameTime);
    }
}