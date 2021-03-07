using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Penumbra;
using System;

public class WorldLight : IGameEntity
{
    public Vector2 Position { get; set; }
    public Size2 Size { get; set; }
    public Light Light { get; set; }

    public WorldLight(Vector2 position, float radius, Color color) {
        this.Position = position;
        this.Size = new Size2(radius, radius);

        Light = new PointLight()
        {
            Position = this.Position,
            Scale = this.Size,
            CastsShadows = true,
            ShadowType = ShadowType.Solid,
            Color = color
        };

        LightingController.AddLight(Light);
    }

    public void Update(float delta) {}

    public void Draw(float delta, SpriteBatch batch) {}

    public void Remove() {
        LightingController.RemoveLight(Light);
        EntityManager.Remove(this);
    }
}