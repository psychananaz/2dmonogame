using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Mono2D.Input;

public class InputState
{
    private KeyboardState _keyboardState;
    private KeyboardState _oldKeyboardState;

    public Vector2 MousePosition { get; private set; }
    public Vector2 MoveDirection { get; private set; }

    public void Update()
    {
        _oldKeyboardState = _keyboardState;
        _keyboardState = Keyboard.GetState();
        MousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
        MoveDirection = GetMoveDirection();
    }

    public bool IsExitRequested()
    {
        return GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || _keyboardState.IsKeyDown(Keys.Escape);
    }

    public bool WasKeyPressed(Keys key)
    {
        return _keyboardState.IsKeyDown(key) && _oldKeyboardState.IsKeyUp(key);
    }

    private Vector2 GetMoveDirection()
    {
        Vector2 direction = Vector2.Zero;
        if (_keyboardState.IsKeyDown(Keys.W) || _keyboardState.IsKeyDown(Keys.Up))
            direction.Y -= 1;
        if (_keyboardState.IsKeyDown(Keys.S) || _keyboardState.IsKeyDown(Keys.Down))
            direction.Y += 1;
        if (_keyboardState.IsKeyDown(Keys.A) || _keyboardState.IsKeyDown(Keys.Left))
            direction.X -= 1;
        if (_keyboardState.IsKeyDown(Keys.D) || _keyboardState.IsKeyDown(Keys.Right))
            direction.X += 1;

        if (direction != Vector2.Zero)
            direction.Normalize();

        return direction;
    }
}
