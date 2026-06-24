using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Intel;

public sealed class IntelDetectorOverlaySystem : EntitySystem
{
	[Dependency]
	private IOverlayManager _overlay;

	public override void Initialize()
	{
		if (!_overlay.HasOverlay<IntelDetectorOverlay>())
		{
			_overlay.AddOverlay((Overlay)(object)new IntelDetectorOverlay());
		}
	}

	public override void Shutdown()
	{
		_overlay.RemoveOverlay<IntelDetectorOverlay>();
	}
}
