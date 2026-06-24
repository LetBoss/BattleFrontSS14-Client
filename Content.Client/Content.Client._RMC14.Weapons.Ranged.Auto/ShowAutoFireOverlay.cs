using System;
using System.Numerics;
using Content.Shared._RMC14.Weapons.Ranged.Auto;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client._RMC14.Weapons.Ranged.Auto;

public sealed class ShowAutoFireOverlay : Overlay
{
	[Dependency]
	private IEntityManager _entity;

	private readonly GunToggleableAutoFireSystem _autoFire;

	public override OverlaySpace Space => (OverlaySpace)8;

	public ShowAutoFireOverlay()
	{
		IoCManager.InjectDependencies<ShowAutoFireOverlay>(this);
		_autoFire = _entity.System<GunToggleableAutoFireSystem>();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		ReadOnlySpan<Vector2> readOnlySpan = _autoFire.Shape.Vertices;
		Color red = Color.Red;
		((DrawingHandleBase)worldHandle).DrawPrimitives((DrawPrimitiveTopology)2, readOnlySpan, ((Color)(ref red)).WithAlpha(0.5f));
	}
}
