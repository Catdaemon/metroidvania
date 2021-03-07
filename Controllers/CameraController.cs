using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using MonoGame.Extended;

public static class CameraController
{
    public static OrthographicCamera Camera;
    private static Vector2 Offset = new Vector2(0,0);
    private static Vector2 Position = new Vector2(0,0);
    private static Vector2 TargetOffset = new Vector2(0,0);
    private static bool YReset = false;
    
    public static void Setup(GraphicsDevice graphicsDevice)
    {
        Camera = new OrthographicCamera(graphicsDevice);
    }

    public static void Update(float delta, Vector2 target, Vector2 moveDirection)
    {
        if (!moveDirection.IsNaN())
        {
            TargetOffset = moveDirection * 32f;
            Offset = new Vector2(MathHelper.Lerp(Offset.X, TargetOffset.X, delta * 0.5f), 0);
        }
        if (Position == Vector2.Zero)
        {
            Position = new Vector2(target.X, target.Y);
        } else {
            Position.X = target.X;
            if (Math.Abs(Position.Y - target.Y) > 32)
            {
                YReset = true;
            }
            if (YReset)
            {
                var correctionSpeed = (Position - target).Length() * 0.1f;
                Position = Position.SetY(MathHelper.Lerp(Position.Y, target.Y, delta * correctionSpeed));
                if (Math.Abs(Position.Y - target.Y) < 4)
                {
                    YReset = false;
                }
            }
        }

        Camera.LookAt(Position + Offset);
        Camera.Zoom = 3f;
    }
}