using System;

namespace Robust.Shared.Timing;

internal interface IGameLoop
{
	bool SingleStep { get; set; }

	bool Running { get; set; }

	int MaxQueuedTicks { get; set; }

	TimeSpan LimitMinFrameTime { get; set; }

	SleepMode SleepMode { get; set; }

	event EventHandler<FrameEventArgs> Input;

	event EventHandler<FrameEventArgs> Tick;

	event EventHandler<FrameEventArgs> Update;

	event EventHandler<FrameEventArgs> Render;

	void Run();
}
