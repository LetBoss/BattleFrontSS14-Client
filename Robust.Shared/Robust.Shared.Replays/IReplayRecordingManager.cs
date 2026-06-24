using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Robust.Shared.Analyzers;
using Robust.Shared.ContentPack;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Markdown.Mapping;

namespace Robust.Shared.Replays;

[NotContentImplementable]
public interface IReplayRecordingManager
{
	bool IsRecording { get; }

	object? ActiveRecordingState { get; }

	event Action<MappingDataNode, List<object>> RecordingStarted;

	event Action<MappingDataNode> RecordingStopped;

	event Action<ReplayRecordingStopped> RecordingStopped2;

	event Action<ReplayRecordingFinished> RecordingFinished;

	bool CanStartRecording();

	void RecordServerMessage(object obj);

	void RecordClientMessage(object obj);

	void RecordReplayMessage(object obj);

	void Update(GameState? state);

	bool TryStartRecording(IWritableDirProvider directory, string? name = null, bool overwrite = false, TimeSpan? duration = null, object? state = null);

	void StopRecording();

	ReplayRecordingStats GetReplayStats();

	Task WaitWriteTasks();

	bool IsWriting();
}
