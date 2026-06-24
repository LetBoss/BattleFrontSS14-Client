using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Emplacements;

public sealed class WeaponMountConeOverlaySystem : EntitySystem
{
	[Dependency]
	private readonly IOverlayManager _overlay;

	public override void Initialize()
	{
		_overlay.AddOverlay((Overlay)(object)new WeaponMountConeOverlay());
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		_overlay.RemoveOverlay<WeaponMountConeOverlay>();
	}
}
