using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._PUBG.Repair;

public sealed class CivRepairStationOverlaySystem : EntitySystem
{
	[Dependency]
	private readonly IOverlayManager _overlay;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		if (!_overlay.HasOverlay<CivRepairStationOverlay>())
		{
			_overlay.AddOverlay((Overlay)(object)new CivRepairStationOverlay());
		}
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		_overlay.RemoveOverlay<CivRepairStationOverlay>();
	}
}
