using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Physics;

public sealed class JointVisualsSystem : EntitySystem
{
	[Dependency]
	private IOverlayManager _overlay;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		_overlay.AddOverlay((Overlay)(object)new JointVisualsOverlay((IEntityManager)(object)base.EntityManager));
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		_overlay.RemoveOverlay<JointVisualsOverlay>();
	}
}
