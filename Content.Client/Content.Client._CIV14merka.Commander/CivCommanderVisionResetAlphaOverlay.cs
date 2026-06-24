using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._CIV14merka.Commander;

public sealed class CivCommanderVisionResetAlphaOverlay : Overlay
{
	[Dependency]
	private IEntityManager _entities;

	private readonly CivCommanderVisionHideSystem _hide;

	public override OverlaySpace Space => (OverlaySpace)4;

	public CivCommanderVisionResetAlphaOverlay()
	{
		IoCManager.InjectDependencies<CivCommanderVisionResetAlphaOverlay>(this);
		_hide = _entities.System<CivCommanderVisionHideSystem>();
	}

	protected override bool BeforeDraw(in OverlayDrawArgs args)
	{
		return _hide.CachedBaseAlphas.Count > 0;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		_hide.RestoreCachedAlphas();
	}
}
