using System;
using System.Collections.Generic;
using Content.Shared._RMC14.PlayTimeTracking;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Client._RMC14.PlayTimeTracking;

public sealed class RMCPlayTimeManager : IPostInjectInit
{
	[Dependency]
	private INetManager _net;

	private readonly HashSet<string> _excluded = new HashSet<string>();

	public event Action? Updated;

	private void OnExcludedTimers(RMCExcludedTimersMsg message)
	{
		_excluded.Clear();
		_excluded.UnionWith(message.Trackers);
		this.Updated?.Invoke();
	}

	public bool IsExcluded(string tracker)
	{
		return _excluded.Contains(tracker);
	}

	void IPostInjectInit.PostInject()
	{
		_net.RegisterNetMessage<RMCExcludedTimersMsg>((ProcessMessage<RMCExcludedTimersMsg>)OnExcludedTimers, (NetMessageAccept)3);
	}
}
