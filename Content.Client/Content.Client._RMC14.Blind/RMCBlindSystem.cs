using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Blind;

public sealed class RMCBlindSystem : EntitySystem
{
	[Dependency]
	private IOverlayManager _overlay;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		_overlay.AddOverlay((Overlay)(object)new RMCBlurOverlay((IEntityManager)(object)base.EntityManager));
	}
}
