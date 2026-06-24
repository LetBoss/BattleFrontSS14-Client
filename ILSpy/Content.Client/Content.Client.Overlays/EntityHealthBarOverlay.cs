using System.Collections.Generic;
using System.Numerics;
using Content.Client.StatusIcon;
using Content.Client.UserInterface.Systems;
using Content.Shared._RMC14.CrashLand;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.ParaDrop;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.Overlays;

public sealed class EntityHealthBarOverlay : Overlay
{
	private readonly IEntityManager _entManager;

	private readonly IPrototypeManager _prototype;

	private readonly SharedTransformSystem _transform;

	private readonly MobStateSystem _mobStateSystem;

	private readonly MobThresholdSystem _mobThresholdSystem;

	private readonly StatusIconSystem _statusIconSystem;

	private readonly SpriteSystem _spriteSystem;

	private readonly ProgressColorSystem _progressColor;

	private readonly EntityQuery<CrashLandingComponent> _crashLandingQuery;

	private readonly EntityQuery<ParaDroppingComponent> _paraDroppingQuery;

	public HashSet<string> DamageContainers = new HashSet<string>();

	public ProtoId<HealthIconPrototype>? StatusIcon;

	public override OverlaySpace Space => (OverlaySpace)8;

	public EntityHealthBarOverlay(IEntityManager entManager, IPrototypeManager prototype)
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		_entManager = entManager;
		_prototype = prototype;
		_transform = _entManager.System<SharedTransformSystem>();
		_mobStateSystem = _entManager.System<MobStateSystem>();
		_mobThresholdSystem = _entManager.System<MobThresholdSystem>();
		_statusIconSystem = _entManager.System<StatusIconSystem>();
		_spriteSystem = _entManager.System<SpriteSystem>();
		_progressColor = _entManager.System<ProgressColorSystem>();
		_crashLandingQuery = _entManager.GetEntityQuery<CrashLandingComponent>();
		_paraDroppingQuery = _entManager.GetEntityQuery<ParaDroppingComponent>();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		IEye eye = args.Viewport.Eye;
		Angle val = ((eye != null) ? eye.Rotation : Angle.Zero);
		EntityQuery<TransformComponent> entityQuery = _entManager.GetEntityQuery<TransformComponent>();
		Vector2 vector = new Vector2(1f, 1f);
		Matrix3x2 value = Matrix3Helpers.CreateScale(ref vector);
		Matrix3x2 value2 = Matrix3Helpers.CreateRotation(Angle.op_Implicit(-val));
		HealthIconPrototype healthIconPrototype = default(HealthIconPrototype);
		_prototype.TryIndex<HealthIconPrototype>(StatusIcon, ref healthIconPrototype);
		AllEntityQueryEnumerator<MobThresholdsComponent, MobStateComponent, DamageableComponent, SpriteComponent> val2 = _entManager.AllEntityQueryEnumerator<MobThresholdsComponent, MobStateComponent, DamageableComponent, SpriteComponent>();
		EntityUid val3 = default(EntityUid);
		MobThresholdsComponent thresholds = default(MobThresholdsComponent);
		MobStateComponent component = default(MobStateComponent);
		DamageableComponent damageableComponent = default(DamageableComponent);
		SpriteComponent val4 = default(SpriteComponent);
		TransformComponent val5 = default(TransformComponent);
		Box2 val8 = default(Box2);
		Box2 val10 = default(Box2);
		Box2 val11 = default(Box2);
		while (val2.MoveNext(ref val3, ref thresholds, ref component, ref damageableComponent, ref val4))
		{
			if ((healthIconPrototype != null && !_statusIconSystem.IsVisible(Entity<MetaDataComponent>.op_Implicit((val3, _entManager.GetComponent<MetaDataComponent>(val3))), healthIconPrototype)) || !entityQuery.TryGetComponent(val3, ref val5) || val5.MapID != args.MapId || !damageableComponent.DamageContainerID.HasValue)
			{
				continue;
			}
			HashSet<string> damageContainers = DamageContainers;
			ProtoId<DamageContainerPrototype>? damageContainerID = damageableComponent.DamageContainerID;
			if (!damageContainers.Contains(damageContainerID.HasValue ? ProtoId<DamageContainerPrototype>.op_Implicit(damageContainerID.GetValueOrDefault()) : null))
			{
				continue;
			}
			Box2 val6 = (Box2)(((_003F?)EntityManagerExt.GetComponentOrNull<StatusIconComponent>(_entManager, val3)?.Bounds) ?? _spriteSystem.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((val3, val4))));
			Vector2 worldPosition = _transform.GetWorldPosition(val5, entityQuery);
			Box2 val7 = ((Box2)(ref val6)).Translated(worldPosition);
			if (!((Box2)(ref val7)).Intersects(ref args.WorldAABB))
			{
				continue;
			}
			(float, bool)? tuple = CalcProgress(val3, component, damageableComponent, thresholds);
			if (tuple.HasValue)
			{
				(float, bool) valueOrDefault = tuple.GetValueOrDefault();
				Matrix3x2 value3 = Matrix3Helpers.CreateTranslation(_transform.GetWorldPosition(val5));
				Matrix3x2 value4 = Matrix3x2.Multiply(value, value3);
				Matrix3x2 matrix3x = Matrix3x2.Multiply(value2, value4);
				((DrawingHandleBase)worldHandle).SetTransform(ref matrix3x);
				float num = ((Box2)(ref val6)).Height * 32f / 2f - 3f;
				float num2 = ((Box2)(ref val6)).Width * 32f;
				Vector2 vector2 = new Vector2((0f - num2) / 32f / 2f, num / 32f);
				Color progressColor = GetProgressColor(valueOrDefault.Item1, valueOrDefault.Item2);
				if (_crashLandingQuery.HasComp(val3) || _paraDroppingQuery.HasComp(val3))
				{
					num = 0.4f + val4.Offset.Y;
					num2 = val4.Offset.X;
					vector2 = new Vector2(num2, num);
				}
				float num3 = num2 - 8f;
				float x = (num3 - 8f) * valueOrDefault.Item1 + 8f;
				((Box2)(ref val8))._002Ector(new Vector2(8f, 0f) / 32f, new Vector2(num3, 3f) / 32f);
				val8 = ((Box2)(ref val8)).Translated(vector2);
				Box2 val9 = val8;
				Color black = Color.Black;
				worldHandle.DrawRect(val9, ((Color)(ref black)).WithAlpha((byte)192), true);
				((Box2)(ref val10))._002Ector(new Vector2(8f, 0f) / 32f, new Vector2(x, 3f) / 32f);
				val10 = ((Box2)(ref val10)).Translated(vector2);
				worldHandle.DrawRect(val10, progressColor, true);
				((Box2)(ref val11))._002Ector(new Vector2(8f, 2f) / 32f, new Vector2(x, 3f) / 32f);
				val11 = ((Box2)(ref val11)).Translated(vector2);
				Box2 val12 = val11;
				black = Color.Black;
				worldHandle.DrawRect(val12, ((Color)(ref black)).WithAlpha((byte)128), true);
			}
		}
		Matrix3x2 identity = Matrix3x2.Identity;
		((DrawingHandleBase)worldHandle).SetTransform(ref identity);
	}

	private (float ratio, bool inCrit)? CalcProgress(EntityUid uid, MobStateComponent component, DamageableComponent dmg, MobThresholdsComponent thresholds)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (_mobStateSystem.IsAlive(uid, component))
		{
			FixedPoint2 totalDamage;
			FixedPoint2? healthBarThreshold;
			if (dmg.HealthBarThreshold.HasValue)
			{
				totalDamage = dmg.TotalDamage;
				healthBarThreshold = dmg.HealthBarThreshold;
				if (totalDamage < healthBarThreshold)
				{
					return null;
				}
			}
			if (!_mobThresholdSystem.TryGetThresholdForState(uid, MobState.Critical, out var threshold, thresholds) && !_mobThresholdSystem.TryGetThresholdForState(uid, MobState.Dead, out threshold, thresholds))
			{
				return (1f, false);
			}
			totalDamage = dmg.TotalDamage;
			healthBarThreshold = threshold;
			return (1f - (totalDamage / healthBarThreshold).Value.Float(), false);
		}
		if (_mobStateSystem.IsCritical(uid, component))
		{
			if (!_mobThresholdSystem.TryGetThresholdForState(uid, MobState.Critical, out var threshold2, thresholds) || !_mobThresholdSystem.TryGetThresholdForState(uid, MobState.Dead, out var threshold3, thresholds))
			{
				return (1f, true);
			}
			FixedPoint2 totalDamage = dmg.TotalDamage;
			FixedPoint2? fixedPoint = threshold2;
			return (1f - ((totalDamage - fixedPoint) / (threshold3 - threshold2)).Value.Float(), true);
		}
		return (0f, true);
	}

	public Color GetProgressColor(float progress, bool crit)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (crit)
		{
			progress = 0f;
		}
		return _progressColor.GetProgressColor(progress);
	}
}
