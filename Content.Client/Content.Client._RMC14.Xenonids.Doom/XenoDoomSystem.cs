using Content.Shared._RMC14.Xenonids.Doom;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Xenonids.Doom;

public sealed class XenoDoomSystem : SharedXenoDoomSystem
{
	[Dependency]
	private IOverlayManager _overlay;

	protected override void OnDoomedAdded(Entity<MobDoomedComponent> ent, ref ComponentStartup args)
	{
		_overlay.AddOverlay((Overlay)(object)new DoomOverlay());
	}

	protected override void OnDoomedRemoved(Entity<MobDoomedComponent> ent, ref ComponentShutdown args)
	{
		_overlay.RemoveOverlay<DoomOverlay>();
	}
}
