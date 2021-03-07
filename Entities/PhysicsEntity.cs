using tainicom.Aether.Physics2D.Dynamics;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Graphics;

public class PhysicsEntity : IGameEntity
{
    public Body Body { get; set; }
    public Size2 Size { get; set; }

    public Vector2 Position {
        get {
            return Body.WorldCenter;
        }
        set {
            Body.Position = value;
        }
    }

    public bool IsGrounded()
    {
        var footPos = this.Position + new Vector2(0, Size.Height / 2);
        var downRay = PhysicsController.RayCastFirst(footPos, footPos + new Vector2(0, 2), this);
        return downRay.Hit;
    }

    public virtual void Update(float delta) {}
    public virtual void Draw(float delta, SpriteBatch batch) {}

    public virtual void Remove() {
        PhysicsController.World.Remove(Body);
        EntityManager.Remove(this);
    }
}