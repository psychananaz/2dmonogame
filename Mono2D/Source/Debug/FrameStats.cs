namespace Mono2D.Debug;

public class FrameStats
{
    private double _timer;
    private int _physicsTicks;
    private int _drawFrames;

    public int FrameCount { get; private set; }
    public double LogicFrequency { get; private set; }
    public double DrawFrequency { get; private set; }

    public void Update(double rawDelta)
    {
        _timer += rawDelta;
        if (_timer < 1.0)
            return;

        LogicFrequency = _physicsTicks / _timer;
        DrawFrequency = _drawFrames / _timer;
        _physicsTicks = 0;
        _drawFrames = 0;
        _timer = 0.0;
    }

    public void CountPhysicsTick()
    {
        _physicsTicks++;
    }

    public void CountDrawFrame()
    {
        FrameCount++;
        _drawFrames++;
    }
}
