using System.Runtime.CompilerServices;

namespace Robust.Shared.Profiling;

public struct ProfBuffer
{
	public long LogWriteOffset;

	public long IndexWriteOffset;

	public ProfLog[] LogBuffer;

	public ProfIndex[] IndexBuffer;

	public readonly ProfBuffer Snapshot()
	{
		ProfBuffer result = this;
		result.LogBuffer = (ProfLog[])result.LogBuffer.Clone();
		result.IndexBuffer = (ProfIndex[])result.IndexBuffer.Clone();
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly ref ProfLog Log(long idx)
	{
		return ref LogBuffer[idx & (LogBuffer.LongLength - 1)];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly ref ProfIndex Index(long idx)
	{
		return ref IndexBuffer[idx & (IndexBuffer.LongLength - 1)];
	}
}
