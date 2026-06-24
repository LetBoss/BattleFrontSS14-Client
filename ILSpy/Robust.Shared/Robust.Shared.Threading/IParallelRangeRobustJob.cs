namespace Robust.Shared.Threading;

public interface IParallelRangeRobustJob
{
	int MinimumBatchParallel => 2;

	int BatchSize => 1;

	void ExecuteRange(int startIndex, int endIndex);
}
