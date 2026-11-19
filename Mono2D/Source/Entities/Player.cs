using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Mono2D.Input;

namespace Mono2D.Entities;

public class Player
{
    private const float Speed = 600;
    private const float Size = 32;

    private Vector2 _previousPosition = new Vector2(200, 200);
    private Color _color = Color.Yellow;

    public Vector2 Position { get; private set; } = new Vector2(200, 200);
    public bool FollowMouse { get; set; }
    public Vector2 MoveDirection { get; set; }
    public Vector2 FollowTarget { get; set; }
    private Vector2 _velocity = Vector2.Zero;

    public void Update(double fixedDelta)
    {
        float dt = (float)fixedDelta;
        _previousPosition = Position;

        if (FollowMouse)
        {
            Position = FollowTarget;
            _velocity = Vector2.Zero;
            return;
        }

        _velocity -= _velocity * 10f * dt;
        if (MoveDirection != Vector2.Zero)
        {
            _velocity += MoveDirection * (Speed * 15f) * dt;
        }
        
        if (_velocity.Length() > Speed)
        {
            _velocity.Normalize();
            _velocity *= Speed;
        }
        
        if (MoveDirection == Vector2.Zero && _velocity.Length() < 1.0f)
        {
            _velocity = Vector2.Zero;
        }
        
        Position += _velocity * (float)fixedDelta;
    }


    public void Draw(SpriteBatch spriteBatch, Texture2D texture, float alpha)
    {
       Console.WriteLine(_velocity);
       
       spriteBatch.Draw(texture, _previousPosition, null, Color.Red, 0.0f, Vector2.Zero,
           Vector2.One * Size, SpriteEffects.None, 0.0f);
        
        // skip interpolation if following the mouse
        var position = FollowMouse
            ? FollowTarget
            : Vector2.Lerp(_previousPosition, Position, alpha);
        spriteBatch.Draw(texture, position, null, _color, 0.0f, Vector2.Zero,
            Vector2.One * Size, SpriteEffects.None, 0.0f);
    }
}
