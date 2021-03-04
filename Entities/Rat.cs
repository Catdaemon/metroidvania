using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;
using tainicom.Aether.Physics2D.Collision;
using MonoGame.Extended;
using System;

public class Rat : PhysicsEntity
{
    private DateTime nextRandomMovement;
    private int moveFrequency;

    private DateTime nextAttack;
    private int attackRate = 3;

    public Rat() {
        this.Size = new Size2(12, 12);
        Body = PhysicsController.World.CreateCircle(Size.Width / 2, 5, bodyType: BodyType.Dynamic);
        Body.Tag = this;

        var rand = new Random();
        moveFrequency = rand.Next(1, 5);
        nextRandomMovement = DateTime.Now.AddSeconds(moveFrequency);

        nextAttack = DateTime.Now.AddSeconds(attackRate);

        Body.SetRestitution(0f);
        Body.FixedRotation = true;
    }

    public override void Update(double delta) {
        var closest = EntityManager.GetClosestPlayer(this.Position);

        if (closest != null)
        {
            var distance = (closest.Position - this.Position).Length();

            if (distance < 64 && nextAttack < DateTime.Now)
            {
                var movement = (closest.Position - this.Position).NormalizedCopy();
                this.Body.LinearVelocity = new Vector2(movement.X * 120, -200);
                nextAttack = DateTime.Now.AddSeconds(attackRate);
                return;
            } else if (distance < 96)
            {
                var movement = (closest.Position - this.Position).NormalizedCopy();
                this.Body.LinearVelocity = new Vector2(movement.X * 80, this.Body.LinearVelocity.Y);
                return;
            }
        }

        // Random idle movement
        if (DateTime.Now > nextRandomMovement)
        {
            var rand = new Random();
            var moveAmount = rand.Next(-300, 300);
            nextRandomMovement = DateTime.Now.AddSeconds(moveFrequency);

            this.Body.LinearVelocity = new Vector2(moveAmount, this.Body.LinearVelocity.Y);
        }
    }

    public override void Draw(double delta, SpriteBatch batch) {
        batch.DrawRectangle(this.Position - new Vector2(Size.Width / 2, Size.Height / 2), Size, Color.Red, 1, 0);
    }
}