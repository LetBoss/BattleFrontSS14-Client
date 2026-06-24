using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Robust.Shared.Replays;

internal sealed class ReplayStatsCommand : LocalizedCommands
{
	[Dependency]
	private readonly IReplayRecordingManager _replay;

	public override string Command => "replay_recording_stats";

	public override string Description => LocalizationManager.GetString("cmd-replay-recording-stats-desc");

	public override string Help => LocalizationManager.GetString("cmd-replay-recording-stats-help");

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		if (_replay.IsRecording)
		{
			ReplayRecordingStats replayStats = _replay.GetReplayStats();
			float num = (float)replayStats.Size / 1048576f;
			double totalMinutes = replayStats.Time.TotalMinutes;
			shell.WriteLine(base.Loc.GetString("cmd-replay-recording-stats-result", ("time", totalMinutes.ToString("F1")), ("ticks", replayStats.Ticks), ("size", num.ToString("F1")), ("rate", ((double)num / totalMinutes).ToString("F2"))));
		}
		else
		{
			shell.WriteLine(base.Loc.GetString("cmd-replay-recording-stop-not-recording"));
		}
	}
}
