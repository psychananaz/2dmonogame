using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Mono2D;

public class CoreEngine : Game
{
    private const double FixedStep = 1.0 / 120.0;
    private const double MaxFrameDelta = 0.25; // avoid running too many updates when rendering is heavy
    private const int RenderLoadSpriteCount = 0;

    private const float PlayerSpeed = 400;
    private const float PlayerSize = 16;

    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SpriteFont _font;
    private Texture2D _debugTexture;

    private KeyboardState _keyboardState;
    private KeyboardState _oldKeyboardState;

    private double _accumulator;

    private double _fpsTimer;
    private int _physicsTicks;
    private int _drawFrames;

    private Vector2 _playerPosition = new Vector2(200, 200);
    private Vector2 _previousPlayerPosition = new Vector2(200, 200);
    private Vector2 _moveDirection;
    private Color _playerColor = Color.Yellow;

    private bool _followMouse;
    private Vector2 _mousePosition;
    private Vector2 _mousePhysicsPosition;

    public int FrameCount { get; private set; }
    public double LogicFrequency { get; private set; }
    public double DrawFrequency { get; private set; }

    public CoreEngine()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = false;
        IsFixedTimeStep = false;
        _graphics.SynchronizeWithVerticalRetrace = false;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _font = Content.Load<SpriteFont>("Fonts/Hud");
        _debugTexture = Content.Load<Texture2D>("Textures/pixel");
    }

    protected override void Update(GameTime gameTime)
    {
        double rawDelta = gameTime.ElapsedGameTime.TotalSeconds;
        double delta = rawDelta > MaxFrameDelta ? MaxFrameDelta : rawDelta;

        UpdateTimers(rawDelta);
        InputUpdate(gameTime);

        _accumulator += delta;
        while (_accumulator >= FixedStep)
        {
            _accumulator -= FixedStep;
            PhysicsUpdate(FixedStep);
        }

        base.Update(gameTime);
    }

    private void UpdateTimers(double rawDelta)
    {
        _fpsTimer += rawDelta;
        if (_fpsTimer < 1.0)
            return;

        LogicFrequency = _physicsTicks / _fpsTimer;
        DrawFrequency = _drawFrames / _fpsTimer;
        _physicsTicks = 0;
        _drawFrames = 0;
        _fpsTimer = 0.0;
    }

    private void PhysicsUpdate(double fixedDelta)
    {
        _physicsTicks++;
        // check if input gets through
        _playerColor = _followMouse ? Color.Red : Color.Yellow;
        _mousePhysicsPosition = _mousePosition;
        MovePlayer(fixedDelta);
    }

    private void MovePlayer(double fixedDelta)
    {
        _previousPlayerPosition = _playerPosition;
        if (_followMouse)
        {
            _playerPosition = _mousePosition;
        } else {
            _playerPosition += _moveDirection * PlayerSpeed * (float)fixedDelta;
        }
    }

    private void InputUpdate(GameTime gameTime)
    {
        _oldKeyboardState = _keyboardState;
        _keyboardState = Keyboard.GetState();
        _mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || _keyboardState.IsKeyDown(Keys.Escape))
            Exit();

        if (WasKeyPressed(Keys.Space))
        {
            _followMouse = !_followMouse;
        }
    
        _moveDirection = GetMoveDirection();
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

    private bool WasKeyPressed(Keys key)
    {
        return _keyboardState.IsKeyDown(key) && _oldKeyboardState.IsKeyUp(key);
    }

    protected override void Draw(GameTime gameTime)
    {
        FrameCount++;
        _drawFrames++;

        float alpha = (float)(_accumulator / FixedStep);

        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
        DrawHud();
        DrawRenderLoad();
        DrawPlayer(alpha);
        DrawCursor();
        DrawPhysicsCursor();
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void DrawHud()
    {
        _spriteBatch.DrawString(_font, $"FRAMES: {FrameCount}", new Vector2(10, 10), Color.White);
        _spriteBatch.DrawString(_font, $"LOGIC: {LogicFrequency:0}", new Vector2(10, 30), Color.White);
        _spriteBatch.DrawString(_font, $"DRAW: {DrawFrequency:0}", new Vector2(10, 50), Color.White);
        _spriteBatch.DrawString(_font, $"POS: {_playerPosition.X:0} {_playerPosition.Y:0}", new Vector2(10, 70), Color.White);
        
    }

    // Simulates heavy rendering so the draw rate drops well below the fixed logic rate.
    private void DrawRenderLoad()
    {
        for (int i = 0; i < RenderLoadSpriteCount; i++)
        {
            _spriteBatch.Draw(_debugTexture, new Vector2(10, 70), null, Color.White, 0.0f, Vector2.Zero, Vector2.One * 10.0f, SpriteEffects.None, 0.0f);
        }
    }


    private void DrawPlayer(float alpha)
    {
        // skip interpolation if following the mouse
        Vector2 position = _followMouse
            ? _mousePosition
            : Vector2.Lerp(_previousPlayerPosition, _playerPosition, alpha);
        _spriteBatch.Draw(_debugTexture, position, null, _playerColor, 0.0f, Vector2.Zero,
            Vector2.One * PlayerSize, SpriteEffects.None, 0.0f);
    }
    
    private void DrawCursor()
    {
        _spriteBatch.Draw(_debugTexture, _mousePosition, null, Color.White, 0.0f, new
            Vector2(0.5f), Vector2.One * 6.0f, SpriteEffects.None, 0.0f);
    }
    
    private void DrawPhysicsCursor()
    {
        _spriteBatch.Draw(_debugTexture, _mousePhysicsPosition, null, Color.Red, 0.0f, new
            Vector2(0.5f), Vector2.One * 6.0f, SpriteEffects.None, 0.0f);
    }
}
