using Content.Shared._RMC14.Marines.Orders;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Marines.Orders;

public sealed class MarineOrdersSystem : SharedMarineOrdersSystem
{
	[Dependency]
	private IOverlayManager _overlays;

	public override void Initialize()
	{
		base.Initialize();
		if (!_overlays.HasOverlay<OrdersOverlay>())
		{
			_overlays.AddOverlay((Overlay)(object)new OrdersOverlay());
		}
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		_overlays.RemoveOverlay<OrdersOverlay>();
	}
}
