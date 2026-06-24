using Content.Shared._RMC14.Vents;
using Robust.Client.Graphics;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.VentCrawl;

public sealed class VentCrawlingSystem : SharedVentCrawlingSystem
{
	[Dependency]
	private IOverlayManager _overlay;

	public override void Initialize()
	{
		base.Initialize();
		if (!_overlay.HasOverlay<VentCrawlIconOverlay>())
		{
			_overlay.AddOverlay((Overlay)(object)new VentCrawlIconOverlay());
		}
	}

	public override void Shutdown()
	{
		_overlay.RemoveOverlay<VentCrawlIconOverlay>();
	}
}
