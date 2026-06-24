using Content.Shared._RMC14.Teleporter;
using Robust.Client.Graphics;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Teleporter;

public sealed class RMCTeleporterSystem : SharedRMCTeleporterSystem
{
	[Dependency]
	private IOverlayManager _overlay;

	public override void Initialize()
	{
		base.Initialize();
		_overlay.AddOverlay((Overlay)(object)new RMCTeleporterViewerOverlay());
	}

	public override void Shutdown()
	{
		_overlay.RemoveOverlay<RMCTeleporterViewerOverlay>();
	}
}
