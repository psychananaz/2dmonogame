using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Arch;

namespace Mono2D;

public class CoreEngine : Game
{
    private const double FixedStep = 1.0 / 60.0;
    private const double MaxFrameDelta = 0.25; // avoid running too many updates when rendering is heavy
    private const int RenderLoadSpriteCount = 1000000;

    private const float PlayerSpeed = 200.0f;
    private const float PlayerSize = 32.0f;

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
    private Color _playerColor = Color.Yellow;
    private int _pressCount;

    public int FrameCount { get; private set; }
    public double LogicFrequency { get; private set; }
    public double DrawFrequency { get; private set; }

    public CoreEngine()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

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

        _keyboardState = Keyboard.GetState();

        UpdateFrequencyCounters(rawDelta);
        RunFixedSteps(delta);
        InputUpdate(gameTime);

        _oldKeyboardState = _keyboardState;
        base.Update(gameTime);
    }

    private void UpdateFrequencyCounters(double rawDelta)
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

    private void RunFixedSteps(double delta)
    {
        _accumulator += delta;
        while (_accumulator >= FixedStep)
        {
            _accumulator -= FixedStep;
            PhysicsUpdate(FixedStep);
        }
    }

    private void PhysicsUpdate(double fixedDelta)
    {
        _physicsTicks++;
        MovePlayer(fixedDelta);
    }

    private void MovePlayer(double fixedDelta)
    {
        _previousPlayerPosition = _playerPosition;

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

        _playerPosition += direction * PlayerSpeed * (float)fixedDelta;
    }

    // Single presses are handled here instead of in PhysicsUpdate because a tap that starts and
    // ends between two fixed steps would never be seen there, and this runs every frame for lower latency.
    private void InputUpdate(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || _keyboardState.IsKeyDown(Keys.Escape))
            Exit();

        if (WasKeyPressed(Keys.Space))
        {
            _pressCount++;
            _playerColor = _playerColor == Color.Yellow ? Color.Red : Color.Yellow;
        }
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

        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();
        DrawHud();
        DrawRenderLoad();
        DrawPlayer(alpha);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void DrawHud()
    {
        _spriteBatch.DrawString(_font, $"FRAMES: {FrameCount}", new Vector2(10, 10), Color.White);
        _spriteBatch.DrawString(_font, $"LOGIC: {LogicFrequency:0}", new Vector2(10, 30), Color.White);
        _spriteBatch.DrawString(_font, $"DRAW: {DrawFrequency:0}", new Vector2(10, 50), Color.White);
        _spriteBatch.DrawString(_font, $"PRESSES: {_pressCount}", new Vector2(10, 90), Color.White);
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
        Vector2 position = Vector2.Lerp(_previousPlayerPosition, _playerPosition, alpha);
        _spriteBatch.Draw(_debugTexture, position, null, _playerColor, 0.0f, Vector2.Zero, Vector2.One * PlayerSize, SpriteEffects.None, 0.0f);
    }
}
