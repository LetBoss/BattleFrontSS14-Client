using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Robust.Shared.Replays;

internal sealed class ReplayStopCommand : LocalizedCommands
{
	[Dependency]
	private readonly IReplayRecordingManager _replay;

	public override string Command => "replay_recording_stop";

	public override string Description => LocalizationManager.GetString("cmd-replay-recording-stop-desc");

	public override string Help => LocalizationManager.GetString("cmd-replay-recording-stop-help");

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		if (_replay.IsRecording)
		{
			_replay.StopRecording();
			shell.WriteLine(base.Loc.GetString("cmd-replay-recording-stop-success"));
		}
		else
		{
			shell.WriteLine(base.Loc.GetString("cmd-replay-recording-stop-not-recording"));
		}
	}
}
