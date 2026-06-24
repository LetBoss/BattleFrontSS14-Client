using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._CIV14merka.Roof;

public sealed class CivRoofCrackOverlaySystem : EntitySystem
{
	[Dependency]
	private IOverlayManager _overlay;

	private CivRoofCrackOverlay? _cracks;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		_cracks = new CivRoofCrackOverlay((IEntityManager)(object)base.EntityManager);
		_overlay.AddOverlay((Overlay)(object)_cracks);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		if (_cracks != null)
		{
			_overlay.RemoveOverlay((Overlay)(object)_cracks);
		}
	}
}
