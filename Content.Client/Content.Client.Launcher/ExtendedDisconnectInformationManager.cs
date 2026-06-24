using System;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Client.Launcher;

public sealed class ExtendedDisconnectInformationManager
{
	[Dependency]
	private IClientNetManager _clientNetManager;

	private NetDisconnectedArgs? _lastNetDisconnectedArgs;

	public NetDisconnectedArgs? LastNetDisconnectedArgs
	{
		get
		{
			return _lastNetDisconnectedArgs;
		}
		private set
		{
			_lastNetDisconnectedArgs = value;
			this.LastNetDisconnectedArgsChanged?.Invoke(value);
		}
	}

	public event Action<NetDisconnectedArgs?>? LastNetDisconnectedArgsChanged;

	public void Initialize()
	{
		((INetManager)_clientNetManager).Disconnect += OnNetDisconnect;
	}

	private void OnNetDisconnect(object? sender, NetDisconnectedArgs args)
	{
		LastNetDisconnectedArgs = args;
	}
}
