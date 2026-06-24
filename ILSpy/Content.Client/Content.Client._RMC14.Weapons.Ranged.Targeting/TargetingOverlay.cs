using System.Numerics;
using Content.Shared._RMC14.Targeting;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client._RMC14.Weapons.Ranged.Targeting;

public sealed class TargetingOverlay : Overlay
{
	private IEntityManager _entManager;

	public override OverlaySpace Space => (OverlaySpace)8;

	public TargetingOverlay(IEntityManager entManager)
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
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<RMCTargetedComponent> val = _entManager.EntityQueryEnumerator<RMCTargetedComponent>();
		EntityQuery<TransformComponent> entityQuery = _entManager.GetEntityQuery<TransformComponent>();
		EntityQuery<TargetingLaserComponent> entityQuery2 = _entManager.GetEntityQuery<TargetingLaserComponent>();
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		SharedTransformSystem val2 = _entManager.System<SharedTransformSystem>();
		EntityUid val3 = default(EntityUid);
		RMCTargetedComponent rMCTargetedComponent = default(RMCTargetedComponent);
		TargetingLaserComponent targetingLaserComponent = default(TargetingLaserComponent);
		TransformComponent val4 = default(TransformComponent);
		TransformComponent val5 = default(TransformComponent);
		Box2 val7 = default(Box2);
		Box2Rotated val8 = default(Box2Rotated);
		while (val.MoveNext(ref val3, ref rMCTargetedComponent))
		{
			foreach (EntityUid item in rMCTargetedComponent.TargetedBy)
			{
				if (!entityQuery2.TryGetComponent(item, ref targetingLaserComponent) || !targetingLaserComponent.ShowLaser || !entityQuery.TryGetComponent(item, ref val4) || !entityQuery.TryGetComponent(val3, ref val5) || val5.MapID != val4.MapID)
				{
					continue;
				}
				Vector2 worldPosition = val2.GetWorldPosition(val5, entityQuery);
				Vector2 worldPosition2 = val2.GetWorldPosition(val4, entityQuery);
				Vector2 vector = worldPosition - worldPosition2;
				Angle val6 = DirectionExtensions.ToWorldAngle(vector);
				float num = vector.Length() / 2f;
				Vector2 vector2 = worldPosition2 + vector / 2f;
				((Box2)(ref val7))._002Ector(-0.02f, 0f - num, 0.02f, num);
				((Box2Rotated)(ref val8))._002Ector(((Box2)(ref val7)).Translated(vector2), val6, vector2);
				Color currentLaserColor = targetingLaserComponent.CurrentLaserColor;
				float num2 = 0f;
				if (targetingLaserComponent.GradualAlpha)
				{
					if (rMCTargetedComponent.AlphaMultipliers.TryGetValue(item, out var value))
					{
						num2 = targetingLaserComponent.LaserAlpha * value;
					}
				}
				else
				{
					num2 = targetingLaserComponent.LaserAlpha;
				}
				worldHandle.DrawRect(ref val8, ((Color)(ref currentLaserColor)).WithAlpha(num2), true);
			}
		}
	}
}
