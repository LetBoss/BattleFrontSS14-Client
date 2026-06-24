using System.Runtime.InteropServices;

namespace Robust.Shared.Profiling;

[StructLayout(LayoutKind.Explicit)]
public struct ProfLog
{
	[FieldOffset(0)]
	public ProfLogType Type;

	[FieldOffset(8)]
	public ProfLogValue Value;

	[FieldOffset(8)]
	public ProfLogGroupEnd GroupEnd;
}
