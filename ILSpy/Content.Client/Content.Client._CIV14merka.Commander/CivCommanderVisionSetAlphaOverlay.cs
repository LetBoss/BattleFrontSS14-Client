using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._CIV14merka.Commander;

public sealed class CivCommanderVisionSetAlphaOverlay : Overlay
{
	[Dependency]
	private IEntityManager _entities;

	private readonly CivCommanderVisionHideSystem _hide;

	private bool _ready;

	public override OverlaySpace Space => (OverlaySpace)64;

	public CivCommanderVisionSetAlphaOverlay()
	{
		IoCManager.InjectDependencies<CivCommanderVisionSetAlphaOverlay>(this);
		_hide = _entities.System<CivCommanderVisionHideSystem>();
	}

	protected override bool BeforeDraw(in OverlayDrawArgs args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		_ready = _hide.Prepare(args.MapId);
		return _ready;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		if (_ready)
		{
			_hide.Apply();
		}
	}
}
