using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Xenonids.Hud;

public sealed class XenoHudSystem : EntitySystem
{
	[Dependency]
	private IOverlayManager _overlay;

	public override void Initialize()
	{
		if (!_overlay.HasOverlay<XenoHudOverlay>())
		{
			_overlay.AddOverlay((Overlay)(object)new XenoHudOverlay());
		}
	}

	public override void Shutdown()
	{
		_overlay.RemoveOverlay<XenoHudOverlay>();
	}
}
