using System;
using System.Numerics;
using Content.Shared._RMC14.Emplacements;
using Content.Shared.Buckle.Components;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Client._RMC14.Emplacements;

public sealed class WeaponMountConeOverlay : Overlay
{
	[Dependency]
	private readonly IEntityManager _entity;

	[Dependency]
	private readonly IPlayerManager _player;

	private readonly SharedTransformSystem _transform;

	private readonly SharedWeaponMountSystem _mount;

	private const float ConeLength = 50f;

	private const float DashLength = 0.4f;

	private const float GapLength = 0.3f;

	private static readonly Color LineColor;

	public override OverlaySpace Space => (OverlaySpace)4;

	public WeaponMountConeOverlay()
	{
		IoCManager.InjectDependencies<WeaponMountConeOverlay>(this);
		_transform = _entity.System<SharedTransformSystem>();
		_mount = _entity.System<SharedWeaponMountSystem>();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		BuckleComponent buckleComponent = default(BuckleComponent);
		if (localEntity.HasValue && _entity.TryGetComponent<BuckleComponent>(localEntity.Value, ref buckleComponent) && buckleComponent.Buckled)
		{
			EntityUid value = buckleComponent.BuckledTo.Value;
			if (_mount.TryGetMountCone(Entity<WeaponMountComponent>.op_Implicit(value), out var shootArc))
			{
				ValueTuple<Vector2, Angle> worldPositionRotation = _transform.GetWorldPositionRotation(value);
				Vector2 item = worldPositionRotation.Item1;
				Angle item2 = worldPositionRotation.Item2;
				Angle val = Angle.FromDegrees((double)shootArc / 2.0);
				Angle angle = item2 + val;
				Angle angle2 = item2 - val;
				DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
				DrawDashedLine(worldHandle, item, angle);
				DrawDashedLine(worldHandle, item, angle2);
			}
		}
	}

	private static void DrawDashedLine(DrawingHandleWorld handle, Vector2 origin, Angle angle)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = ((Angle)(ref angle)).ToWorldVec();
		float num = 0.70000005f;
		for (float num2 = 0f; num2 < 50f; num2 += num)
		{
			Vector2 vector2 = origin + vector * num2;
			float num3 = MathF.Min(num2 + 0.4f, 50f);
			Vector2 vector3 = origin + vector * num3;
			((DrawingHandleBase)handle).DrawLine(vector2, vector3, LineColor);
		}
	}

	static WeaponMountConeOverlay()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Color white = Color.White;
		LineColor = ((Color)(ref white)).WithAlpha(0.25f);
	}
}
