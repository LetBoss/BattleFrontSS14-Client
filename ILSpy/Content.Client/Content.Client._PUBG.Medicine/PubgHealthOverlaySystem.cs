using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._PUBG.Medicine;

public sealed class PubgHealthOverlaySystem : EntitySystem
{
	[Dependency]
	private IOverlayManager _overlay;

	private PubgHealthOverlay? _healthOverlay;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		_healthOverlay = new PubgHealthOverlay();
		_overlay.AddOverlay((Overlay)(object)_healthOverlay);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		if (_healthOverlay != null)
		{
			_overlay.RemoveOverlay((Overlay)(object)_healthOverlay);
		}
	}
}
