using System.Diagnostics.Metrics;

namespace Robust.Shared.Utility;

internal static class ResizableMemoryRegionMetrics
{
	public static readonly Meter Meter = new Meter("Robust.ResizableMemoryRegion");

	public const string GaugeName = "used_bytes";
}
