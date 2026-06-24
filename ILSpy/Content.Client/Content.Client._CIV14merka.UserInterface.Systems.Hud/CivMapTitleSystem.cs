using System;
using Content.Shared._CIV14merka;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._CIV14merka.UserInterface.Systems.Hud;

public sealed class CivMapTitleSystem : EntitySystem
{
	[Dependency]
	private IOverlayManager _overlayManager;

	private CivMapTitleOverlay _overlay;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<CivMapTitleEvent>((EntitySessionEventHandler<CivMapTitleEvent>)OnMapTitle, (Type[])null, (Type[])null);
		_overlay = new CivMapTitleOverlay();
		_overlayManager.AddOverlay((Overlay)(object)_overlay);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		_overlayManager.RemoveOverlay((Overlay)(object)_overlay);
	}

	private void OnMapTitle(CivMapTitleEvent msg, EntitySessionEventArgs args)
	{
		_overlay.Show(msg.Title);
	}
}
