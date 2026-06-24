using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._CIV14merka.Atgm;

public sealed class CivAtgmWireOverlaySystem : EntitySystem
{
	[Dependency]
	private readonly IOverlayManager _overlays;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		if (!_overlays.HasOverlay<CivAtgmWireOverlay>())
		{
			_overlays.AddOverlay((Overlay)(object)new CivAtgmWireOverlay());
		}
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		_overlays.RemoveOverlay<CivAtgmWireOverlay>();
	}
}
