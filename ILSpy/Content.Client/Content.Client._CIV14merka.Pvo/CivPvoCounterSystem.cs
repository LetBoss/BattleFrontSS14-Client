using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._CIV14merka.Pvo;

public sealed class CivPvoCounterSystem : EntitySystem
{
	[Dependency]
	private readonly IOverlayManager _overlays;

	private CivPvoCounterOverlay? _overlay;

	public override void Initialize()
	{
		_overlay = new CivPvoCounterOverlay();
		_overlays.AddOverlay((Overlay)(object)_overlay);
	}

	public override void Shutdown()
	{
		if (_overlay != null)
		{
			_overlays.RemoveOverlay((Overlay)(object)_overlay);
			_overlay = null;
		}
	}
}
