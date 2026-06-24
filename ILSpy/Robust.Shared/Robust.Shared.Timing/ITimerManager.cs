using System.Threading;
using Robust.Shared.Analyzers;

namespace Robust.Shared.Timing;

[NotContentImplementable]
public interface ITimerManager
{
	void AddTimer(Timer timer, CancellationToken cancellationToken = default(CancellationToken));

	void UpdateTimers(FrameEventArgs frameEventArgs);
}
