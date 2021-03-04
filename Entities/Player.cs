using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;
using tainicom.Aether.Physics2D.Collision;
using MonoGame.Extended;
using System;

public class Player : PhysicsEntity
{
    private bool grounded = false;
    private float maxSpeed = 100;
    
    public Player() {
        this.Size = new Size2(8, 24);
        Body = PhysicsController.World.CreateCapsule(Size.Height, Size.Width, 4, Size.Width, 100, 1, bodyType: BodyType.Dynamic);
        Body.Tag = this;

        Body.Mass = 5;
        Body.SetRestitution(0f);
        Body.FixedRotation = true;
        Body.OnCollision += onCollision;
        Body.OnSeparation += onSeparated;
    }

    public override void Update(double delta) {
        this.grounded = this.IsGrounded();

        var airResist = 2f;
        var groundResist = 10f;

        if (grounded) {
            Body.LinearVelocity = new Vector2(MathHelper.Lerp(Body.LinearVelocity.X, 0,groundResist *(float)delta), 0);

            Body.LinearVelocity = new Vector2(Body.LinearVelocity.X, 0);
            if (InputController.Jump) {
                Body.LinearVelocity = new Vector2(Body.LinearVelocity.X, -100f);
            }

            float moveSpeed = (1000f * InputController.X * (float)delta);
            if (Body.LinearVelocity.X < maxSpeed) {
                Body.LinearVelocity = new Vector2(Body.LinearVelocity.X + moveSpeed, Body.LinearVelocity.Y);
            }
        } else {
            Body.LinearVelocity = new Vector2(MathHelper.Lerp(Body.LinearVelocity.X, 0, airResist *(float)delta), Body.LinearVelocity.Y);

            float moveSpeed = (500f * InputController.X * (float)delta);
            if (Body.LinearVelocity.X < maxSpeed) {
                Body.LinearVelocity = new Vector2(Body.LinearVelocity.X + moveSpeed, Body.LinearVelocity.Y);
            }
            //Body.LinearVelocity = new Vector2(Body.LinearVelocity.X, Body.LinearVelocity.Y + gravity);
        }
    
        //var upRay = PhysicsController.RayCastFirst(this.Position, this.Position - new Vector2(0, (Size.Height / 2) + 4), this);
        //var leftRay = PhysicsController.RayCastFirst(this.Position, this.Position - new Vector2((Size.Width / 2) + 8, 0), this);
        //var rightRay = PhysicsController.RayCastFirst(this.Position, this.Position + new Vector2((Size.Width / 2) + 8, 0), this);

        // if (upRay.Hit) {
        //     var hitPos = upRay.Point;
        //     Body.Position = new Vector2(Body.Position.X, hitPos.Y + (Size.Height * 0.5f));
        //     Body.LinearVelocity = new Vector2(Body.LinearVelocity.X, 400);
        // }

        // if (Body.LinearVelocity.X < 0 && leftRay.Hit || Body.LinearVelocity.X > 0 && rightRay.Hit) {
        //     Body.LinearVelocity = new Vector2(0, Body.LinearVelocity.Y);
        // }

        var movement = this.Body.LinearVelocity * (float)delta;
        var newPos = this.Position + movement;

        base.Update(delta);
        
    }

    public override void Draw(double delta, SpriteBatch batch) {
        if (grounded) {
            batch.DrawRectangle(this.Position - new Vector2(Size.Width / 2, Size.Height / 2), Size, Color.Red, 1, 0);
        } else {
            batch.DrawRectangle(this.Position - new Vector2(Size.Width / 2, Size.Height / 2), Size, Color.Blue, 1, 0);
        }

        var movement = this.Body.LinearVelocity * (float)delta;
        var newPos = this.Position + movement;
        var moveAngle = this.Body.LinearVelocity.NormalizedCopy();
        AABB bbox;
        this.Body.FixtureList[0].GetAABB(out bbox, 0);

        var rayStart = this.Position + (moveAngle * bbox.Extents);

        batch.DrawLine(rayStart, rayStart + (moveAngle * 4), Color.Red, 1);

        base.Draw(delta, batch);
    }

    public bool onCollision(Fixture sender, Fixture other, Contact contact)
    {
        return true;
    }

    public void onSeparated(Fixture sender, Fixture other, Contact contact)
    {
        
    }
}