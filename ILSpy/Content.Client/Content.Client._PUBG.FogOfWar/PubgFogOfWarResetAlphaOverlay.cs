using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client._PUBG.FogOfWar;

public sealed class PubgFogOfWarResetAlphaOverlay : Overlay
{
	[Dependency]
	private IEntityManager _ent;

	private readonly PubgFogOfWarHideSystem _hide;

	private readonly SpriteSystem _sprite;

	public override OverlaySpace Space => (OverlaySpace)4;

	public PubgFogOfWarResetAlphaOverlay()
	{
		IoCManager.InjectDependencies<PubgFogOfWarResetAlphaOverlay>(this);
		_hide = _ent.System<PubgFogOfWarHideSystem>();
		_sprite = _ent.System<SpriteSystem>();
	}

	protected override bool BeforeDraw(in OverlayDrawArgs args)
	{
		if (_hide.CachedBaseAlphas.Count == 0)
		{
			return false;
		}
		return true;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		foreach (var (val, num) in _hide.CachedBaseAlphas)
		{
			if (val.Comp != null)
			{
				SpriteSystem sprite = _sprite;
				Entity<SpriteComponent> val2 = val;
				Color color = val.Comp.Color;
				sprite.SetColor(val2, ((Color)(ref color)).WithAlpha(num));
			}
		}
		_hide.CachedBaseAlphas.Clear();
	}
}
