using Content.Shared._RMC14.Marines;
using Robust.Client.Graphics;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Marines;

public sealed class MarineSystem : SharedMarineSystem
{
	[Dependency]
	private IOverlayManager _overlays;

	public override void Initialize()
	{
		base.Initialize();
		_overlays.AddOverlay((Overlay)(object)new MarineOverlay());
	}
}
