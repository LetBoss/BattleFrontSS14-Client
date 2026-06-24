using Content.Shared.Inventory.Events;
using Content.Shared.Overlays;
using Robust.Client.Graphics;
using Robust.Shared.IoC;

namespace Content.Client.Overlays;

public sealed class BlackAndWhiteOverlaySystem : EquipmentHudSystem<BlackAndWhiteOverlayComponent>
{
	[Dependency]
	private IOverlayManager _overlayMan;

	private BlackAndWhiteOverlay _overlay;

	public override void Initialize()
	{
		base.Initialize();
		_overlay = new BlackAndWhiteOverlay();
	}

	protected override void UpdateInternal(RefreshEquipmentHudEvent<BlackAndWhiteOverlayComponent> component)
	{
		base.UpdateInternal(component);
		_overlayMan.AddOverlay((Overlay)(object)_overlay);
	}

	protected override void DeactivateInternal()
	{
		base.DeactivateInternal();
		_overlayMan.RemoveOverlay((Overlay)(object)_overlay);
	}
}
