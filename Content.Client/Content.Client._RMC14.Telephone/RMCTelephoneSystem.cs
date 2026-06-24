using Content.Shared._RMC14.Telephone;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Telephone;

public sealed class RMCTelephoneSystem : SharedRMCTelephoneSystem
{
	[Dependency]
	private IOverlayManager _overlay;

	public override void Initialize()
	{
		base.Initialize();
		if (!_overlay.HasOverlay<TelephoneOverlay>())
		{
			_overlay.AddOverlay((Overlay)(object)new TelephoneOverlay());
		}
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		_overlay.RemoveOverlay<TelephoneOverlay>();
	}
}
