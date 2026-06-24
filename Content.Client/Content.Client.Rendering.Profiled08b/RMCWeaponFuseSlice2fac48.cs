using Robust.Client.Graphics;
using Robust.Shared.Enums;

namespace Content.Client.Rendering.Profiled08b;

public sealed class RMCWeaponFuseSlice2fac48 : Overlay
{
	private readonly RMCProfileCacheNodea4fdbc _fba75b59e508b;

	public override OverlaySpace Space => (OverlaySpace)8;

	public RMCWeaponFuseSlice2fac48(RMCProfileCacheNodea4fdbc weapon)
	{
		_fba75b59e508b = weapon;
	}

	protected override bool BeforeDraw(in OverlayDrawArgs args)
	{
		return _fba75b59e508b._m9988195534c0();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		_fba75b59e508b._m7902fc32fa5f();
	}
}
