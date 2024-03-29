using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

public interface IGameEntity
{
    public Vector2 Position { get; set; }
    public Size2 Size { get; set; }

    public void Draw(float delta, SpriteBatch batch);
    public void Update(float delta);
    public void Remove();
}