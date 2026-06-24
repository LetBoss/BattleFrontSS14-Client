using System;
using System.Threading;
using Robust.Shared.Analyzers;

namespace Robust.Shared.Threading;

[NotContentImplementable]
public interface IParallelManager
{
	int ParallelProcessCount { get; }

	event Action ParallelCountChanged;

	void AddAndInvokeParallelCountChanged(Action changed);

	WaitHandle Process(IRobustJob job);

	void ProcessNow(IRobustJob job);

	void ProcessNow(IParallelRobustJob jobs, int amount);

	void ProcessSerialNow(IParallelRobustJob jobs, int amount);

	WaitHandle Process(IParallelRobustJob jobs, int amount);

	void ProcessNow(IParallelBulkRobustJob jobs, int amount);

	void ProcessSerialNow(IParallelBulkRobustJob jobs, int amount);

	WaitHandle Process(IParallelBulkRobustJob jobs, int amount);
}
