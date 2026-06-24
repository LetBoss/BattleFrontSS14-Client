using Content.Shared.Inventory.Events;
using Content.Shared.Overlays;
using Robust.Client.Graphics;
using Robust.Shared.IoC;

namespace Content.Client.Overlays;

public sealed class NoirOverlaySystem : EquipmentHudSystem<NoirOverlayComponent>
{
	[Dependency]
	private IOverlayManager _overlayMan;

	private NoirOverlay _overlay;

	public override void Initialize()
	{
		base.Initialize();
		_overlay = new NoirOverlay();
	}

	protected override void UpdateInternal(RefreshEquipmentHudEvent<NoirOverlayComponent> component)
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
