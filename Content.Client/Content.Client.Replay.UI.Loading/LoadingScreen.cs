using System;
using Robust.Client.ResourceManagement;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.CPUJob.JobQueues;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Client.Replay.UI.Loading;

[Virtual]
public class LoadingScreen<TResult> : State
{
	[Dependency]
	private IResourceCache _resourceCache;

	[Dependency]
	private IUserInterfaceManager _userInterfaceManager;

	private LoadingScreenControl _screen;

	public Job<TResult>? Job;

	public event Action<TResult?, Exception?>? OnJobFinished;

	public override void FrameUpdate(FrameEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Invalid comparison between Unknown and I4
		((State)this).FrameUpdate(e);
		if (Job != null)
		{
			Job.Run();
			if ((int)Job.Status == 4)
			{
				this.OnJobFinished?.Invoke(Job.Result, Job.Exception);
				Job = null;
			}
		}
	}

	protected override void Startup()
	{
		_screen = new LoadingScreenControl(_resourceCache);
		((Control)_userInterfaceManager.StateRoot).AddChild((Control)(object)_screen);
	}

	protected override void Shutdown()
	{
		((Control)_screen).Orphan();
	}

	public void UpdateProgress(float value, float maxValue, string header, string subtext = "")
	{
		((Range)_screen.Bar).Value = value;
		((Range)_screen.Bar).MaxValue = maxValue;
		_screen.Header.Text = header;
		_screen.Subtext.Text = subtext;
	}
}
