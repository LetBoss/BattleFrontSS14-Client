using System;
using System.Collections.Generic;
using Content.Shared._PUBG.Lobby;
using Robust.Shared.GameObjects;

namespace Content.Client._PUBG.Lobby;

public sealed class LobbyPermissionsSystem : EntitySystem
{
	public Dictionary<string, int> Permissions { get; private set; } = new Dictionary<string, int>();

	public event Action<Dictionary<string, int>>? OnPermissionsReceived;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<LobbyPermissionsMessage>((EntityEventHandler<LobbyPermissionsMessage>)OnLobbyPermissions, (Type[])null, (Type[])null);
	}

	private void OnLobbyPermissions(LobbyPermissionsMessage msg)
	{
		Permissions = new Dictionary<string, int>(msg.Permissions);
		this.OnPermissionsReceived?.Invoke(Permissions);
	}

	public void RequestPermissions()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new LobbyPermissionsRequestMessage());
	}

	public void ClearPermissions()
	{
		Permissions.Clear();
	}
}
