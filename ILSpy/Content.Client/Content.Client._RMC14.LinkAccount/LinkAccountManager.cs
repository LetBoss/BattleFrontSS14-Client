using System;
using System.Collections.Generic;
using Content.Shared._RMC14.LinkAccount;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;

namespace Content.Client._RMC14.LinkAccount;

public sealed class LinkAccountManager : IPostInjectInit
{
	[Dependency]
	private INetManager _net;

	private readonly List<SharedRMCPatron> _allPatrons = new List<SharedRMCPatron>();

	public SharedRMCPatronTier? Tier { get; private set; }

	public bool Linked { get; private set; }

	public Color? GhostColor { get; private set; }

	public SharedRMCLobbyMessage? LobbyMessage { get; private set; }

	public SharedRMCRoundEndShoutouts? RoundEndShoutout { get; private set; }

	public event Action<Guid>? CodeReceived;

	public event Action? Updated;

	private void OnCode(LinkAccountCodeMsg message)
	{
		this.CodeReceived?.Invoke(message.Code);
	}

	private void OnStatus(LinkAccountStatusMsg ev)
	{
		Tier = ev.Patron?.Tier;
		Linked = ev.Patron?.Linked ?? false;
		GhostColor = ev.Patron?.GhostColor;
		LobbyMessage = ev.Patron?.LobbyMessage;
		RoundEndShoutout = ev.Patron?.RoundEndShoutout;
		this.Updated?.Invoke();
	}

	private void OnPatronList(RMCPatronListMsg ev)
	{
		_allPatrons.Clear();
		_allPatrons.AddRange(ev.Patrons);
	}

	public IReadOnlyList<SharedRMCPatron> GetPatrons()
	{
		return _allPatrons;
	}

	public bool CanViewPatronPerks()
	{
		SharedRMCPatronTier tier = Tier;
		if ((object)tier != null)
		{
			if (!tier.GhostColor && !tier.NamedItems && !tier.Figurines && !tier.LobbyMessage)
			{
				return tier.RoundEndShoutout;
			}
			return true;
		}
		return false;
	}

	void IPostInjectInit.PostInject()
	{
		_net.RegisterNetMessage<LinkAccountCodeMsg>((ProcessMessage<LinkAccountCodeMsg>)OnCode, (NetMessageAccept)3);
		_net.RegisterNetMessage<LinkAccountRequestMsg>((ProcessMessage<LinkAccountRequestMsg>)null, (NetMessageAccept)3);
		_net.RegisterNetMessage<LinkAccountStatusMsg>((ProcessMessage<LinkAccountStatusMsg>)OnStatus, (NetMessageAccept)3);
		_net.RegisterNetMessage<RMCPatronListMsg>((ProcessMessage<RMCPatronListMsg>)OnPatronList, (NetMessageAccept)3);
		_net.RegisterNetMessage<RMCClearGhostColorMsg>((ProcessMessage<RMCClearGhostColorMsg>)null, (NetMessageAccept)3);
		_net.RegisterNetMessage<RMCChangeGhostColorMsg>((ProcessMessage<RMCChangeGhostColorMsg>)null, (NetMessageAccept)3);
		_net.RegisterNetMessage<RMCChangeLobbyMessageMsg>((ProcessMessage<RMCChangeLobbyMessageMsg>)null, (NetMessageAccept)3);
		_net.RegisterNetMessage<RMCChangeMarineShoutoutMsg>((ProcessMessage<RMCChangeMarineShoutoutMsg>)null, (NetMessageAccept)3);
		_net.RegisterNetMessage<RMCChangeXenoShoutoutMsg>((ProcessMessage<RMCChangeXenoShoutoutMsg>)null, (NetMessageAccept)3);
	}
}
