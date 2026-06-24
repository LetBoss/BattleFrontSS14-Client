namespace Robust.Shared.Replays;

internal interface IReplayRecordingManagerInternal : IReplayRecordingManager
{
	void Initialize();

	void Shutdown();
}
