using System;
using Robust.Client.UserInterface;
using Robust.Shared.Timing;

namespace Content.Client.Items.UI;

public abstract class PollingItemStatusControl<TData> : Control where TData : struct, IEquatable<TData>
{
	private TData _lastData;

	protected override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).FrameUpdate(args);
		TData data = PollData();
		if (!data.Equals(_lastData))
		{
			_lastData = data;
			Update(in data);
		}
	}

	protected abstract TData PollData();

	protected abstract void Update(in TData data);
}
