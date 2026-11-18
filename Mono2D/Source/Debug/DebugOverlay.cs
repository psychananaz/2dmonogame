using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mono2D.Debug;

public class DebugOverlay
{
    private const int RenderLoadSpriteCount = 0;

    private readonly SpriteFont _font;
    private readonly Texture2D _texture;

    public DebugOverlay(SpriteFont font, Texture2D texture)
    {
        _font = font;
        _texture = texture;
    }

    public void DrawDebugHud(SpriteBatch spriteBatch, FrameStats stats, Vector2 playerPosition)
    {
        spriteBatch.DrawString(_font, $"FRAMES: {stats.FrameCount}", new Vector2(10, 10), Color.White);
        spriteBatch.DrawString(_font, $"LOGIC: {stats.LogicFrequency:0}", new Vector2(10, 30), Color.White);
        spriteBatch.DrawString(_font, $"DRAW: {stats.DrawFrequency:0}", new Vector2(10, 50), Color.White);
        spriteBatch.DrawString(_font, $"POS: {playerPosition.X:0} {playerPosition.Y:0}", new Vector2(10, 70), Color.White);
    }

    public void DrawCursor(SpriteBatch spriteBatch, Vector2 position, Color color)
    {
        spriteBatch.Draw(_texture, position, null, color, 0.0f, new
            Vector2(0.5f), Vector2.One * 6.0f, SpriteEffects.None, 0.0f);
    }
}
