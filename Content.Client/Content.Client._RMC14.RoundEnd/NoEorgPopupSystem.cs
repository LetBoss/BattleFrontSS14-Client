using System;
using Content.Shared.GameTicking;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.RoundEnd;

public sealed class NoEorgPopupSystem : EntitySystem
{
	private NoEorgPopup? _window;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<RoundEndMessageEvent>((EntityEventHandler<RoundEndMessageEvent>)OnRoundEnd, (Type[])null, (Type[])null);
	}

	private void OnRoundEnd(RoundEndMessageEvent ev)
	{
	}

	private void OpenNoEorgPopup()
	{
		if (_window == null)
		{
			_window = new NoEorgPopup();
			((BaseWindow)_window).OpenCentered();
			((BaseWindow)_window).OnClose += delegate
			{
				_window = null;
			};
		}
	}
}
