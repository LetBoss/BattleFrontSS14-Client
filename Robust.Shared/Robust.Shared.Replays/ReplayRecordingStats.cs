using System;

namespace Robust.Shared.Replays;

public record struct ReplayRecordingStats(TimeSpan Time, uint Ticks, long Size, long UncompressedSize);
