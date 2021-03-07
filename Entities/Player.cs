using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;
using tainicom.Aether.Physics2D.Collision;
using MonoGame.Extended;
using System;
using Spine;
using Penumbra;

public class Player : PhysicsEntity, ISkeletalEntity
{
    private bool grounded = false;
    private bool isAimingForward = true;
    private float maxSpeed = 100;

    private Vector2 AimAngle = new Vector2(0,0);

    public Skeleton Skeleton { get; set; }
    public AnimationState AnimationState { get; set; }
    public string CurrentAnimation { get; set; } = "";

    Bone ShootPos;
    Bone GunPos;
    Bone GunArm;
    Light Flashlight;
    
    public Player() {
        this.Size = new Size2(8, 24);
        Body = PhysicsController.World.CreateCapsule(Size.Height, Size.Width, 4, Size.Width, 4, 1, bodyType: BodyType.Dynamic);
        Body.Tag = this;

        Body.Mass = 5;
        Body.SetRestitution(0f);
        Body.FixedRotation = true;
        Body.OnCollision += onCollision;
        Body.OnSeparation += onSeparated;

        (this as ISkeletalEntity).InitSkeleton("test/spineboy.atlas", "test/spineboy-pro.skel");
        ShootPos = Skeleton.FindBone("gun-tip");
        GunPos = Skeleton.FindBone("gun");
        GunArm = Skeleton.FindBone("rear-upper-arm");

        Flashlight = new Spotlight
            {
                Scale = new Vector2(500f), // Range of the light source (how far the light will travel),
                //ConeDecay = 0.25f,
                ShadowType = ShadowType.Occluded, // Will not lit hulls themselves
                Position = new Vector2(500,200),
                Rotation = 0
            };
        LightingController.AddLight(Flashlight);
    }

    public override void Update(float delta) {
        this.grounded = this.IsGrounded();
        AimAngle = InputController.GetAimNormal(this.Position);

        var airResist = 2f;
        var groundResist = 10f;

        if (grounded) {
            Body.LinearVelocity = new Vector2(MathHelper.Lerp(Body.LinearVelocity.X, 0,groundResist *delta), 0);

            Body.LinearVelocity = new Vector2(Body.LinearVelocity.X, 0);
            if (InputController.Jump) {
                Body.LinearVelocity = new Vector2(Body.LinearVelocity.X, -100f);
            }

            float moveSpeed = (1000f * InputController.X * delta);
            if (Body.LinearVelocity.X < maxSpeed) {
                Body.LinearVelocity = new Vector2(Body.LinearVelocity.X + moveSpeed, Body.LinearVelocity.Y);
            }
        } else {
            Body.LinearVelocity = new Vector2(MathHelper.Lerp(Body.LinearVelocity.X, 0, airResist *delta), Body.LinearVelocity.Y);

            float moveSpeed = (500f * InputController.X * delta);
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

        var movement = this.Body.LinearVelocity * delta;
        var newPos = this.Position + movement;

        isAimingForward = AimAngle.X > 0;
        
        if (grounded && InputController.X > 0) {
            (this as ISkeletalEntity).SetSkeletonAnimation(0, "walk", false, true);
        } else if (grounded && InputController.X < 0 ) {
            (this as ISkeletalEntity).SetSkeletonAnimation(0, "walk", false, true);
        } else if (this.Body.LinearVelocity.Y < 0) {
            (this as ISkeletalEntity).SetSkeletonAnimation(0, "jump", false, true);
        } else {
            (this as ISkeletalEntity).SetSkeletonAnimation(0, "idle", false, true);
        }

        Skeleton.ScaleX = isAimingForward ? Math.Abs(Skeleton.ScaleX) : Math.Abs(Skeleton.ScaleX) * -1;

        base.Update(delta);
        
    }

    public override void Draw(float delta, SpriteBatch batch) {

        var _shootPos = new Vector2(ShootPos.worldX, ShootPos.worldY);

        batch.DrawLine(_shootPos, _shootPos + (AimAngle * 32), Color.Red, 1);

        //batch.End();

        (this as ISkeletalEntity).BeforeDrawSkeleton(delta);

        GunArm.rotation = MathHelper.ToDegrees(AimAngle.ToAngle()) - 44;
        if (isAimingForward) {
            GunArm.rotation = MathHelper.ToDegrees((AimAngle * new Vector2(-1, 1)).ToAngle()) - 44;
        }

        (this as ISkeletalEntity).DrawSkeleton(delta);

        Flashlight.Position = _shootPos;
        Flashlight.Rotation = AimAngle.ToAngle() - 1.5f;
        //light.Rotation = LocalPlayer.Body.LinearVelocity.ToAngle() - 1.5f;

        //batch.Begin();

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