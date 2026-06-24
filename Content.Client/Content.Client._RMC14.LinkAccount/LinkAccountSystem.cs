using System;
using Content.Shared._RMC14.LinkAccount;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.LinkAccount;

public sealed class LinkAccountSystem : EntitySystem
{
	public event Action<SharedRMCDisplayLobbyMessageEvent>? LobbyMessageReceived;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeNetworkEvent<SharedRMCDisplayLobbyMessageEvent>((EntityEventHandler<SharedRMCDisplayLobbyMessageEvent>)OnDisplayLobbyMessage, (Type[])null, (Type[])null);
	}

	private void OnDisplayLobbyMessage(SharedRMCDisplayLobbyMessageEvent ev)
	{
		this.LobbyMessageReceived?.Invoke(ev);
	}
}
