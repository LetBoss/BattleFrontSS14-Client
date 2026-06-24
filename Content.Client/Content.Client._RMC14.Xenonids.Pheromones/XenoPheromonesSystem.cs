using Content.Shared._RMC14.Xenonids.Pheromones;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Xenonids.Pheromones;

public sealed class XenoPheromonesSystem : SharedXenoPheromonesSystem
{
	[Dependency]
	private IOverlayManager _overlays;

	public override void Initialize()
	{
		base.Initialize();
		_overlays.AddOverlay((Overlay)(object)new XenoPheromonesOverlay());
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		_overlays.RemoveOverlay<XenoPheromonesOverlay>();
	}
}
