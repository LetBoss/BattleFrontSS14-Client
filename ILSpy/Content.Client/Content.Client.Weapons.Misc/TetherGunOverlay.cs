using System.Numerics;
using Content.Shared.Weapons.Misc;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Weapons.Misc;

public sealed class TetherGunOverlay : Overlay
{
	private IEntityManager _entManager;

	public override OverlaySpace Space => (OverlaySpace)8;

	public TetherGunOverlay(IEntityManager entManager)
	{
		_entManager = entManager;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<TetheredComponent> val = _entManager.EntityQueryEnumerator<TetheredComponent>();
		EntityQuery<TransformComponent> entityQuery = _entManager.GetEntityQuery<TransformComponent>();
		EntityQuery<TetherGunComponent> entityQuery2 = _entManager.GetEntityQuery<TetherGunComponent>();
		EntityQuery<ForceGunComponent> entityQuery3 = _entManager.GetEntityQuery<ForceGunComponent>();
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		SharedTransformSystem val2 = _entManager.System<SharedTransformSystem>();
		EntityUid val3 = default(EntityUid);
		TetheredComponent tetheredComponent = default(TetheredComponent);
		TransformComponent val4 = default(TransformComponent);
		TransformComponent val5 = default(TransformComponent);
		Box2 val7 = default(Box2);
		Box2Rotated val8 = default(Box2Rotated);
		ForceGunComponent forceGunComponent = default(ForceGunComponent);
		TetherGunComponent tetherGunComponent = default(TetherGunComponent);
		while (val.MoveNext(ref val3, ref tetheredComponent))
		{
			EntityUid tetherer = tetheredComponent.Tetherer;
			if (entityQuery.TryGetComponent(tetherer, ref val4) && entityQuery.TryGetComponent(val3, ref val5) && !(val5.MapID != val4.MapID))
			{
				Vector2 worldPosition = val2.GetWorldPosition(val5, entityQuery);
				Vector2 worldPosition2 = val2.GetWorldPosition(val4, entityQuery);
				Vector2 vector = worldPosition - worldPosition2;
				Angle val6 = DirectionExtensions.ToWorldAngle(vector);
				float num = vector.Length() / 2f;
				Vector2 vector2 = worldPosition2 + vector / 2f;
				((Box2)(ref val7))._002Ector(-0.05f, 0f - num, 0.05f, num);
				((Box2Rotated)(ref val8))._002Ector(((Box2)(ref val7)).Translated(vector2), val6, vector2);
				Color val9 = Color.Red;
				if (entityQuery3.TryGetComponent(tetheredComponent.Tetherer, ref forceGunComponent))
				{
					val9 = forceGunComponent.LineColor;
				}
				else if (entityQuery2.TryGetComponent(tetheredComponent.Tetherer, ref tetherGunComponent))
				{
					val9 = tetherGunComponent.LineColor;
				}
				worldHandle.DrawRect(ref val8, ((Color)(ref val9)).WithAlpha(0.3f), true);
			}
		}
	}
}
