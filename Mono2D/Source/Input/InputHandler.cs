using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Mono2D.Input;

public class InputHandler
{
    private KeyboardState _keyboardState;
    private KeyboardState _oldKeyboardState;
    private GamePadState _gamePadState;

    public Vector2 MousePosition { get; private set; }

    public void Update()
    {
        _oldKeyboardState = _keyboardState;
        _keyboardState = Keyboard.GetState();
        _gamePadState = GamePad.GetState(PlayerIndex.One);

        MouseState mouseState = Mouse.GetState();
        MousePosition = new Vector2(mouseState.X, mouseState.Y);
    }

    public bool IsKeyDown(Keys key)
    {
        return _keyboardState.IsKeyDown(key);
    }

    public bool WasKeyPressed(Keys key)
    {
        return _keyboardState.IsKeyDown(key) && _oldKeyboardState.IsKeyUp(key);
    }

    public bool IsButtonDown(Buttons button)
    {
        return _gamePadState.IsButtonDown(button);
    }
}
