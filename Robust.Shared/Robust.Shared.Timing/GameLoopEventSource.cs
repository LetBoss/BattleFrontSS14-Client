using System.Diagnostics.Tracing;

namespace Robust.Shared.Timing;

[EventSource(Name = "Robust.GameLoop")]
internal sealed class GameLoopEventSource : EventSource
{
	public static GameLoopEventSource Log { get; } = new GameLoopEventSource();

	[Event(1)]
	public void CannotKeepUp()
	{
		WriteEvent(1);
	}

	[Event(2)]
	public void InputStart()
	{
		WriteEvent(2);
	}

	[Event(3)]
	public void InputStop()
	{
		WriteEvent(3);
	}

	[Event(4)]
	public void TickStart(uint tick)
	{
		WriteEvent(4, tick);
	}

	[Event(5)]
	public void TickStop(uint tick)
	{
		WriteEvent(5, tick);
	}

	[Event(6)]
	public void UpdateStart()
	{
		WriteEvent(6);
	}

	[Event(7)]
	public void UpdateStop()
	{
		WriteEvent(7);
	}

	[Event(8)]
	public void SleepStart()
	{
		WriteEvent(8);
	}

	[Event(9)]
	public void SleepStop()
	{
		WriteEvent(9);
	}
}
