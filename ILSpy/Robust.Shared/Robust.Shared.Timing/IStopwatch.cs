using System;

namespace Robust.Shared.Timing;

public interface IStopwatch
{
	TimeSpan Elapsed { get; }

	void Restart();

	void Start();
}
