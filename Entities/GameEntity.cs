using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

public interface IGameEntity
{
    public Vector2 Position { get; set; }
    public Size2 Size { get; set; }

    public void Draw(double delta, SpriteBatch batch);
    public void Update(double delta);
    public void OnDestroyed() {}
}