using System;
using Content.Shared.Speech.EntitySystems;
using Content.Shared.StatusEffect;
using Content.Shared.Traits.Assorted;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Drunk;

public abstract class SharedDrunkSystem : EntitySystem
{
	public static readonly ProtoId<StatusEffectPrototype> DrunkKey = ProtoId<StatusEffectPrototype>.op_Implicit("Drunk");

	[Dependency]
	private StatusEffectsSystem _statusEffectsSystem;

	[Dependency]
	private SharedSlurredSystem _slurredSystem;

	public void TryApplyDrunkenness(EntityUid uid, float boozePower, bool applySlur = true, StatusEffectsComponent? status = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<StatusEffectsComponent>(uid, ref status, false))
		{
			LightweightDrunkComponent trait = default(LightweightDrunkComponent);
			if (((EntitySystem)this).TryComp<LightweightDrunkComponent>(uid, ref trait))
			{
				boozePower *= trait.BoozeStrengthMultiplier;
			}
			if (applySlur)
			{
				_slurredSystem.DoSlur(uid, TimeSpan.FromSeconds(boozePower), status);
			}
			if (!_statusEffectsSystem.HasStatusEffect(uid, ProtoId<StatusEffectPrototype>.op_Implicit(DrunkKey), status))
			{
				_statusEffectsSystem.TryAddStatusEffect<DrunkComponent>(uid, ProtoId<StatusEffectPrototype>.op_Implicit(DrunkKey), TimeSpan.FromSeconds(boozePower), true, status, false);
			}
			else
			{
				_statusEffectsSystem.TryAddTime(uid, ProtoId<StatusEffectPrototype>.op_Implicit(DrunkKey), TimeSpan.FromSeconds(boozePower), status);
			}
		}
	}

	public void TryRemoveDrunkenness(EntityUid uid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_statusEffectsSystem.TryRemoveStatusEffect(uid, ProtoId<StatusEffectPrototype>.op_Implicit(DrunkKey));
	}

	public void TryRemoveDrunkenessTime(EntityUid uid, double timeRemoved)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_statusEffectsSystem.TryRemoveTime(uid, ProtoId<StatusEffectPrototype>.op_Implicit(DrunkKey), TimeSpan.FromSeconds(timeRemoved));
	}
}
