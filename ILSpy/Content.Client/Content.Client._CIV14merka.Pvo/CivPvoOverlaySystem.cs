using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client._CIV14merka.Pvo;

public sealed class CivPvoOverlaySystem : EntitySystem
{
	[Dependency]
	private IOverlayManager _overlayManager;

	[Dependency]
	private IPrototypeManager _prototype;

	private CivPvoOverlay? _overlay;

	public bool Enabled { get; private set; }

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		_overlay = new CivPvoOverlay((IEntityManager)(object)base.EntityManager, base.EntityManager.System<SharedTransformSystem>(), _prototype);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		SetEnabled(enabled: false);
	}

	public bool Toggle()
	{
		SetEnabled(!Enabled);
		return Enabled;
	}

	public void SetEnabled(bool enabled)
	{
		if (_overlay == null || Enabled == enabled)
		{
			return;
		}
		Enabled = enabled;
		if (enabled)
		{
			if (!_overlayManager.HasOverlay<CivPvoOverlay>())
			{
				_overlayManager.AddOverlay((Overlay)(object)_overlay);
			}
		}
		else if (_overlayManager.HasOverlay<CivPvoOverlay>())
		{
			_overlayManager.RemoveOverlay((Overlay)(object)_overlay);
		}
	}
}
