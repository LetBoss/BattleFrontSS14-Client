using System;
using Content.Shared._CIV14merka;
using Robust.Client.Player;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Client._CIV14merka.RoundEnd;

public sealed class CivRoundEndClientSystem : EntitySystem
{
	[Dependency]
	private readonly IPlayerManager _player;

	private CivRoundEndWindow? _window;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<CivRoundEndMessageEvent>((EntitySessionEventHandler<CivRoundEndMessageEvent>)OnRoundEndMessage, (Type[])null, (Type[])null);
	}

	private void OnRoundEndMessage(CivRoundEndMessageEvent ev, EntitySessionEventArgs args)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		CivRoundEndWindow? window = _window;
		if (window != null)
		{
			((BaseWindow)window).Close();
		}
		_window = new CivRoundEndWindow();
		_window.SetTitleText(ev.Title);
		_window.SetSummary(ev.Summary);
		CivRoundEndWindow? window2 = _window;
		ICommonSession localSession = ((ISharedPlayerManager)_player).LocalSession;
		window2.SetMyStats((localSession != null) ? new NetUserId?(localSession.UserId) : ((NetUserId?)null), ev.TeamEntries);
		_window.SetTeamEntries(ev.TeamEntries);
		((BaseWindow)_window).OpenCentered();
	}
}
