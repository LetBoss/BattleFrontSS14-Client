using System;
using System.Collections.Generic;
using Content.Shared._PUBG.Party;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;

namespace Content.Client._PUBG.Party;

public sealed class PubgAdminPartySystem : EntitySystem
{
	public event Action<IReadOnlyList<PubgAdminPartyPlayerInfo>>? PartyListUpdated;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<PubgAdminPartyListResponseEvent>((EntityEventHandler<PubgAdminPartyListResponseEvent>)OnPartyListResponse, (Type[])null, (Type[])null);
	}

	public void RequestPartyList()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgAdminPartyListRequestEvent());
	}

	public void RequestBreakParty(NetUserId targetUserId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgAdminPartyBreakRequestEvent(targetUserId));
	}

	public void RequestForceJoin(NetUserId targetUserId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgAdminPartyForceJoinRequestEvent(targetUserId));
	}

	private void OnPartyListResponse(PubgAdminPartyListResponseEvent ev)
	{
		this.PartyListUpdated?.Invoke(ev.Players);
	}
}
