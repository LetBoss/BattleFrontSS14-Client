using System;
using Content.Shared._PUBG.RoundEnd;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;

namespace Content.Client._PUBG.RoundEnd;

public sealed class PubgRoundEndClientSystem : EntitySystem
{
	private PubgInstanceRoundEndWindow? _window;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<PubgInstanceRoundEndMessageEvent>((EntitySessionEventHandler<PubgInstanceRoundEndMessageEvent>)OnRoundEndMessage, (Type[])null, (Type[])null);
	}

	private void OnRoundEndMessage(PubgInstanceRoundEndMessageEvent ev, EntitySessionEventArgs args)
	{
		PubgInstanceRoundEndWindow? window = _window;
		if (window != null)
		{
			((BaseWindow)window).Close();
		}
		_window = new PubgInstanceRoundEndWindow();
		_window.SetTitleText(ev.Title);
		_window.SetRoundEndText(ev.RoundEndText);
		_window.SetPartyEntries(ev.PartyEntries);
		((BaseWindow)_window).OpenCentered();
	}
}
