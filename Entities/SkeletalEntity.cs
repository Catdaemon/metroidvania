using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Spine;


public interface ISkeletalEntity : IGameEntity
{
    Skeleton Skeleton { get; set; }
    AnimationState AnimationState { get; set; }
    string CurrentAnimation { get; set; }

    public Vector2 SkeletonPosition { 
        get {
            return this.Position + new Vector2(0, this.Size.Height / 2);
        }
    }

    public void InitSkeleton(string atlasPath, string SkeletonPath)
    {
        var atlas = SkeletonController.GetAtlas(atlasPath);
        Skeleton = SkeletonController.GetSkeleton(SkeletonPath, atlas);

        Skeleton.ScaleX = 0.05f;
        Skeleton.ScaleY = 0.05f;

        AnimationStateData stateData = new AnimationStateData(Skeleton.Data);
        stateData.DefaultMix = 0.2f;
        AnimationState = new AnimationState(stateData);
    }

    void BeforeDrawSkeleton(float delta)
    {
        Skeleton.X = this.SkeletonPosition.X;
        Skeleton.Y = this.SkeletonPosition.Y;

        AnimationState.Update(delta);
        AnimationState.Apply(Skeleton);
        
    }

    void DrawSkeleton(float delta)
    {
        Skeleton.UpdateWorldTransform();

        SkeletonController.renderer.Begin();
        SkeletonController.renderer.Draw(Skeleton);
        SkeletonController.renderer.End();
    }

    void SetSkeletonAnimation(int track, string name, bool reset, bool loop)
    {
        if (name != CurrentAnimation || reset) {
            AnimationState.SetAnimation(track, name, loop);
        }

        CurrentAnimation = name;
    }
}