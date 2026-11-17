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
    
    /// <summary>
    /// Returns true if the key is currently down.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool IsKeyDown(Keys key)
    {
        return _keyboardState.IsKeyDown(key);
    }
    
    /// <summary>
    /// Returns true if the key was pressed this frame.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool WasKeyPressed(Keys key)
    {
        return _keyboardState.IsKeyDown(key) && _oldKeyboardState.IsKeyUp(key);
    }
    
    /// <summary>
    /// Returns true if the key was released this frame.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool WasKeyReleased(Keys key)
    {
        return _keyboardState.IsKeyUp(key) && _oldKeyboardState.IsKeyDown(key);
    }
    
    /// <summary>
    /// Returns true if the button is currently down.
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
    public bool IsButtonDown(Buttons button)
    {
        return _gamePadState.IsButtonDown(button);
    }
}
