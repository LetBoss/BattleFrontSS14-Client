using System;
using System.Collections.Generic;
using Content.Shared._PUBG.Lobby;
using Robust.Shared.GameObjects;

namespace Content.Client._PUBG.Lobby;

public sealed class LobbyTraitItemsSystem : EntitySystem
{
	public HashSet<string> UnlockedTraitItems { get; private set; } = new HashSet<string>();

	public event Action<HashSet<string>>? OnTraitItemsReceived;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<LobbyTraitItemsMessage>((EntityEventHandler<LobbyTraitItemsMessage>)OnTraitItems, (Type[])null, (Type[])null);
	}

	private void OnTraitItems(LobbyTraitItemsMessage msg)
	{
		UnlockedTraitItems = new HashSet<string>(msg.UnlockedTraitItems);
		this.OnTraitItemsReceived?.Invoke(new HashSet<string>(UnlockedTraitItems));
	}

	public void RequestTraitItems()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new LobbyTraitItemsRequestMessage());
	}

	public void ClearTraitItems()
	{
		UnlockedTraitItems.Clear();
	}
}
