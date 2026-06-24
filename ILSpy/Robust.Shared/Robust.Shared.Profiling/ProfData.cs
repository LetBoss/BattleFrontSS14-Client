namespace Robust.Shared.Profiling;

public static class ProfData
{
	public static ProfValue Int32(int int32)
	{
		return new ProfValue
		{
			Type = ProfValueType.Int32,
			Int32 = int32
		};
	}

	public static ProfValue Int64(long int64)
	{
		return new ProfValue
		{
			Type = ProfValueType.Int64,
			Int64 = int64
		};
	}

	public static ProfValue TimeAlloc(in ProfSampler sampler)
	{
		return new ProfValue
		{
			Type = ProfValueType.TimeAllocSample,
			TimeAllocSample = new TimeAndAllocSample
			{
				Alloc = sampler.ElapsedAlloc,
				Time = (float)sampler.Elapsed.TotalSeconds
			}
		};
	}
}
