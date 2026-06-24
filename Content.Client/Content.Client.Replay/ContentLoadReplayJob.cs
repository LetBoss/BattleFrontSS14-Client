using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Content.Client.Replay.UI.Loading;
using Robust.Client.Replays.Loading;
using Robust.Shared.Localization;

namespace Content.Client.Replay;

public sealed class ContentLoadReplayJob : LoadReplayJob
{
	private readonly LoadingScreen<bool> _screen;

	public ContentLoadReplayJob(float maxTime, IReplayFileReader fileReader, IReplayLoadManager loadMan, LoadingScreen<bool> screen)
		: base(maxTime, fileReader, loadMan)
	{
		_screen = screen;
	}

	protected override async Task Yield(float value, float maxValue, LoadingState state, bool force)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		string header = Loc.GetString("replay-loading", new(string, object)[2]
		{
			("cur", state + 1),
			("total", 5)
		});
		string subtext = Loc.GetString((int)state switch
		{
			0 => "replay-loading-reading", 
			1 => "replay-loading-processing", 
			2 => "replay-loading-spawning", 
			3 => "replay-loading-initializing", 
			_ => "replay-loading-starting", 
		});
		_screen.UpdateProgress(value, maxValue, header, subtext);
		await _003C_003En__0(value, maxValue, state, force);
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private Task _003C_003En__0(float value, float maxValue, LoadingState state, bool force)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return ((LoadReplayJob)this).Yield(value, maxValue, state, force);
	}
}
