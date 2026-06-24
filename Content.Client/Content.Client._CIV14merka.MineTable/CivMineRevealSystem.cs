using System;
using Content.Shared._CIV14merka.MineTable;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._CIV14merka.MineTable;

public sealed class CivMineRevealSystem : EntitySystem
{
	[Dependency]
	private readonly IOverlayManager _overlays;

	private CivMineRevealOverlay? _overlay;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		_overlay = new CivMineRevealOverlay();
		_overlays.AddOverlay((Overlay)(object)_overlay);
		((EntitySystem)this).SubscribeNetworkEvent<CivMineRevealSnapshotEvent>((EntityEventHandler<CivMineRevealSnapshotEvent>)OnSnapshot, (Type[])null, (Type[])null);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		if (_overlay != null)
		{
			_overlays.RemoveOverlay((Overlay)(object)_overlay);
		}
		_overlay = null;
	}

	private void OnSnapshot(CivMineRevealSnapshotEvent ev)
	{
		if (_overlay != null)
		{
			_overlay.Entries = ev.Entries;
		}
	}
}
