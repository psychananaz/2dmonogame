using System.Diagnostics;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Mono2D.Controllers;
using Mono2D.Debug;
using Mono2D.Entities;
using Mono2D.Input;

namespace Mono2D;

public class CoreEngine : Game
{
    private const double FixedStep = 1.0 / 120.0;
    private const double MaxFrameDelta = 0.25; // avoid running too many updates when rendering is heavy

    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _debugTexture;
    private DebugOverlay _debugOverlay;

    public static InputHandler _input = new();
    private readonly FrameStats _stats = new();
    private readonly Player _player = new();
    private readonly PlayerController _playerController;

    private double _accumulator;
    private Vector2 _mousePhysicsPosition;

    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private double _nextDrawTime;
    private readonly double _maxFps = 200.0;   // 0 = uncapped

    public CoreEngine()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = false;
        IsFixedTimeStep = false;
        _graphics.SynchronizeWithVerticalRetrace = false;

        _playerController = new PlayerController(_input, _player);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _debugTexture = Content.Load<Texture2D>("Textures/pixel");
        _debugOverlay = new DebugOverlay(Content.Load<SpriteFont>("Fonts/Hud"), _debugTexture);
    }
    
    private void LimitDrawRate()
    {
        if (_maxFps <= 0.0) return;

        double target = 1.0 / _maxFps;
        double now = _clock.Elapsed.TotalSeconds;

        if (_nextDrawTime == 0.0) _nextDrawTime = now;

        while (_clock.Elapsed.TotalSeconds < _nextDrawTime)
        {
            double remaining = _nextDrawTime - _clock.Elapsed.TotalSeconds;
            if (remaining > 0.002) Thread.Sleep(1);
            else Thread.SpinWait(100); // just spinwait if there is smol time remaining
        }

        now = _clock.Elapsed.TotalSeconds;
        _nextDrawTime += target;
        if (_nextDrawTime < now) _nextDrawTime = now;  // fell behind, don't accumulate debt
    }

    protected override void Update(GameTime gameTime)
    {
        var rawDelta = gameTime.ElapsedGameTime.TotalSeconds;
        var delta = rawDelta > MaxFrameDelta ? MaxFrameDelta : rawDelta;

        _stats.Update(rawDelta);
        UiInputUpdate();

        _accumulator += delta;
        while (_accumulator >= FixedStep)
        {
            _accumulator -= FixedStep;
            PhysicsUpdate(FixedStep);
        }

        base.Update(gameTime);
    }

    private void PhysicsUpdate(double fixedDelta)
    {
        _stats.CountPhysicsTick();
        _mousePhysicsPosition = _input.MousePosition;
        _player.Update(fixedDelta);
    }

    private void UiInputUpdate()
    {
        _input.Update();

        if (_input.IsKeyDown(Keys.Escape) || _input.IsButtonDown(Buttons.Back))
            Exit();

        _playerController.Update();
    }

    protected override void Draw(GameTime gameTime)
    {
        LimitDrawRate();
        _stats.CountDrawFrame();

        var alpha = (float)(_accumulator / FixedStep);

        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
        
        _debugOverlay.DrawDebugHud(_spriteBatch, _stats, _player.Position);
        
        _player.Draw(_spriteBatch, _debugTexture, alpha);
        
        // draw mouse on "real" position and another on the "fake" position
        // to show the difference between the two
        _debugOverlay.DrawCursor(_spriteBatch, _input.MousePosition, Color.White);
        _debugOverlay.DrawCursor(_spriteBatch, _mousePhysicsPosition, Color.Red);
        
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
