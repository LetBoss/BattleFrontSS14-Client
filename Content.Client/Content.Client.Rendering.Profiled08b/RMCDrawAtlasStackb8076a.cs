using Robust.Client.Graphics;
using Robust.Shared.Enums;

namespace Content.Client.Rendering.Profiled08b;

public sealed class RMCDrawAtlasStackb8076a : Overlay
{
	private readonly RMCProfileCacheNodea4fdbc _feea0b3351e4f;

	public override OverlaySpace Space => (OverlaySpace)64;

	public RMCDrawAtlasStackb8076a(RMCProfileCacheNodea4fdbc weapon)
	{
		_feea0b3351e4f = weapon;
	}

	protected override bool BeforeDraw(in OverlayDrawArgs args)
	{
		return _feea0b3351e4f._mdb8dd0aca972();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		_feea0b3351e4f._m5691d410b8b1();
	}
}
