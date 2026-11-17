using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mono2D.Entities;

public class Player
{
    private const float Speed = 400;
    private const float Size = 16;

    private Vector2 _previousPosition = new Vector2(200, 200);
    private Color _color = Color.Yellow;

    public Vector2 Position { get; private set; } = new Vector2(200, 200);
    public bool FollowMouse { get; set; }

    public void Update(double fixedDelta, Vector2 moveDirection, Vector2 mousePosition)
    {
        // check if input gets through
        _color = FollowMouse ? Color.Red : Color.Yellow;

        _previousPosition = Position;
        if (FollowMouse)
        {
            Position = mousePosition;
        } else {
            Position += moveDirection * Speed * (float)fixedDelta;
        }
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D texture, float alpha, Vector2 mousePosition)
    {
        // skip interpolation if following the mouse
        Vector2 position = FollowMouse
            ? mousePosition
            : Vector2.Lerp(_previousPosition, Position, alpha);
        spriteBatch.Draw(texture, position, null, _color, 0.0f, Vector2.Zero,
            Vector2.One * Size, SpriteEffects.None, 0.0f);
    }
}
