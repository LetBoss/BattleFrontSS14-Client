namespace Robust.Shared.Timing;

public readonly struct FrameEventArgs
{
	public float DeltaSeconds { get; }

	public FrameEventArgs(float deltaSeconds)
	{
		DeltaSeconds = deltaSeconds;
	}
}
