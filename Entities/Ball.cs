using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

public class Ball : PhysicsEntity
{
    public Penumbra.Hull lightingHull;
    public Ball() {
        this.Size = new Size2(8,8);
        Body = PhysicsController.World.CreateCircle(this.Size.Width / 2, 10f, new Vector2(0,0), BodyType.Dynamic);
        Body.SetRestitution(0f);
        Body.Tag = this;

        var _points = new List<Vector2>()
        {
            new Vector2(-this.Size.Width / 2,-this.Size.Height / 2),
            new Vector2(this.Size.Width / 2,-this.Size.Height / 2),
            new Vector2(this.Size.Width / 2,this.Size.Height / 2),
            new Vector2(-this.Size.Width / 2,this.Size.Height / 2),
        };
        lightingHull = new Penumbra.Hull(_points);

        LightingController.AddHull(lightingHull);
    }

    public override void Update(float delta) {
        lightingHull.Position = this.Position;
    }

    public override void Draw(float delta, SpriteBatch batch) {
        var angVec = new Vector2((float)Math.Cos(this.Body.Rotation), (float)Math.Sin(this.Body.Rotation));
        var centre = this.Position;
        batch.DrawLine(centre, centre + (angVec * 4), Color.Red, 1, 0);
        batch.DrawCircle(centre, this.Size.Width / 2, 32, Color.White, 1, 0);
    }

    public override void Remove()
    {
        LightingController.RemoveHull(lightingHull);
        base.Remove();
    }
}