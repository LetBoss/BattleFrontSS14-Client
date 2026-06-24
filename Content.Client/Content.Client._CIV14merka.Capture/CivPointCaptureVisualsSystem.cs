using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._CIV14merka.Capture;

public sealed class CivPointCaptureVisualsSystem : EntitySystem
{
	[Dependency]
	private IOverlayManager _overlayManager;

	[Dependency]
	private IPlayerManager _player;

	private CivPointCaptureOverlay? _overlay;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		_overlay = new CivPointCaptureOverlay((IEntityManager)(object)base.EntityManager, _player);
		_overlayManager.AddOverlay((Overlay)(object)_overlay);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		if (_overlay != null)
		{
			_overlayManager.RemoveOverlay((Overlay)(object)_overlay);
			_overlay = null;
		}
	}
}
