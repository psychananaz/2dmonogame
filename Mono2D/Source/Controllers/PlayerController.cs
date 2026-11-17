using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Mono2D.Entities;
using Mono2D.Input;

namespace Mono2D.Controllers;

public class PlayerController
{
    private readonly InputHandler _input;
    private readonly Player _player;

    public PlayerController(InputHandler input, Player player)
    {
        _input = input;
        _player = player;
    }

    // Runs once per frame rather than per physics tick so key presses toggle exactly once.
    public void Update()
    {
        if (_input.WasKeyPressed(Keys.Space))
            _player.FollowMouse = !_player.FollowMouse;

        _player.MoveDirection = GetMoveDirection();
        _player.FollowTarget = _input.MousePosition;
    }

    private Vector2 GetMoveDirection()
    {
        Vector2 direction = Vector2.Zero;
        if (_input.IsKeyDown(Keys.W) || _input.IsKeyDown(Keys.Up))
            direction.Y -= 1;
        if (_input.IsKeyDown(Keys.S) || _input.IsKeyDown(Keys.Down))
            direction.Y += 1;
        if (_input.IsKeyDown(Keys.A) || _input.IsKeyDown(Keys.Left))
            direction.X -= 1;
        if (_input.IsKeyDown(Keys.D) || _input.IsKeyDown(Keys.Right))
            direction.X += 1;

        if (direction != Vector2.Zero)
            direction.Normalize();

        return direction;
    }
}
