using System.Runtime.InteropServices;

namespace Robust.Shared.Profiling;

[StructLayout(LayoutKind.Explicit)]
public struct ProfValue
{
	[FieldOffset(0)]
	public ProfValueType Type;

	[FieldOffset(8)]
	public TimeAndAllocSample TimeAllocSample;

	[FieldOffset(8)]
	public int Int32;

	[FieldOffset(8)]
	public long Int64;
}
