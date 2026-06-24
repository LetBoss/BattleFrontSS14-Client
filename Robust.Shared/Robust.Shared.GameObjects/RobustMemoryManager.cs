using System;
using System.IO;
using Microsoft.IO;

namespace Robust.Shared.GameObjects;

internal sealed class RobustMemoryManager
{
	private static readonly RecyclableMemoryStreamManager MemStreamManager = new RecyclableMemoryStreamManager(new Options
	{
		ThrowExceptionOnToArray = true
	});

	public RobustMemoryManager()
	{
		MemStreamManager.StreamDoubleDisposed += delegate
		{
			throw new InvalidOperationException("Found double disposed stream.");
		};
		MemStreamManager.StreamFinalized += delegate
		{
			throw new InvalidOperationException("Stream finalized but not disposed indicating a leak");
		};
		MemStreamManager.StreamOverCapacity += delegate
		{
			throw new InvalidOperationException("Stream over memory capacity");
		};
	}

	public static MemoryStream GetMemoryStream()
	{
		return (MemoryStream)(object)MemStreamManager.GetStream("RobustMemoryManager");
	}

	public static MemoryStream GetMemoryStream(int length)
	{
		return (MemoryStream)(object)MemStreamManager.GetStream("RobustMemoryManager", (long)length);
	}
}
