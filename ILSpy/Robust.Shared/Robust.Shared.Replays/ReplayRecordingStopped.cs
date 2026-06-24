using Robust.Shared.Serialization.Markdown.Mapping;

namespace Robust.Shared.Replays;

public sealed class ReplayRecordingStopped
{
	public required MappingDataNode Metadata { get; init; }

	public required IReplayFileWriter Writer { get; init; }

	internal ReplayRecordingStopped()
	{
	}
}
