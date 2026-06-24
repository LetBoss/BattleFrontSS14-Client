using System;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Rounding;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.Xenonids.Damage;

public sealed class RMCXenoDamageVisualsSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private MobThresholdSystem _thresholds;

	private EntityQuery<MobThresholdsComponent> _mobThresholdsQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_mobThresholdsQuery = ((EntitySystem)this).GetEntityQuery<MobThresholdsComponent>();
		((EntitySystem)this).SubscribeLocalEvent<RMCXenoDamageVisualsComponent, DamageChangedEvent>((EntityEventRefHandler<RMCXenoDamageVisualsComponent, DamageChangedEvent>)OnVisualsDamageChanged, (Type[])null, (Type[])null);
	}

	private void OnVisualsDamageChanged(Entity<RMCXenoDamageVisualsComponent> ent, ref DamageChangedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		MobThresholdsComponent thresholds = default(MobThresholdsComponent);
		if (_mobThresholdsQuery.TryComp(Entity<RMCXenoDamageVisualsComponent>.op_Implicit(ent), ref thresholds) && _thresholds.TryGetIncapThreshold(Entity<RMCXenoDamageVisualsComponent>.op_Implicit(ent), out var threshold, thresholds))
		{
			double damage = args.Damageable.TotalDamage.Double();
			double max = threshold.Value.Double();
			FixedPoint2 value = damage;
			FixedPoint2? fixedPoint = threshold;
			int level = ((!(value > fixedPoint)) ? ContentHelpers.RoundToEqualLevels(damage, max, ent.Comp.States + 1) : (ent.Comp.States + 1));
			_appearance.SetData(Entity<RMCXenoDamageVisualsComponent>.op_Implicit(ent), (Enum)RMCDamageVisuals.State, (object)level, (AppearanceComponent)null);
		}
	}
}
