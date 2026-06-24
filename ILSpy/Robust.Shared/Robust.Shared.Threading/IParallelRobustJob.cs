namespace Robust.Shared.Threading;

public interface IParallelRobustJob : IParallelRangeRobustJob
{
	void IParallelRangeRobustJob.ExecuteRange(int startIndex, int endIndex)
	{
		for (int i = startIndex; i < endIndex; i++)
		{
			Execute(i);
		}
	}

	void Execute(int index);
}
