using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.Alert;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Nutrition.Components;
using Content.Shared.Rejuvenate;
using Content.Shared.StatusIcon;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared.Nutrition.EntitySystems;

public sealed class ThirstSystem : EntitySystem
{
	private static readonly bool DisableThirstMovespeedPenalty = true;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private AlertsSystem _alerts;

	[Dependency]
	private MovementSpeedModifierSystem _movement;

	[Dependency]
	private SharedJetpackSystem _jetpack;

	private static readonly ProtoId<SatiationIconPrototype> ThirstIconOverhydratedId = ProtoId<SatiationIconPrototype>.op_Implicit("ThirstIconOverhydrated");

	private static readonly ProtoId<SatiationIconPrototype> ThirstIconThirstyId = ProtoId<SatiationIconPrototype>.op_Implicit("ThirstIconThirsty");

	private static readonly ProtoId<SatiationIconPrototype> ThirstIconParchedId = ProtoId<SatiationIconPrototype>.op_Implicit("ThirstIconParched");

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ThirstComponent, RefreshMovementSpeedModifiersEvent>((ComponentEventHandler<ThirstComponent, RefreshMovementSpeedModifiersEvent>)OnRefreshMovespeed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ThirstComponent, MapInitEvent>((ComponentEventHandler<ThirstComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ThirstComponent, RejuvenateEvent>((ComponentEventHandler<ThirstComponent, RejuvenateEvent>)OnRejuvenate, (Type[])null, (Type[])null);
	}

	private void OnMapInit(EntityUid uid, ThirstComponent component, MapInitEvent args)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (component.CurrentThirst < 0f)
		{
			component.CurrentThirst = _random.Next((int)component.ThirstThresholds[ThirstThreshold.Thirsty] + 10, (int)component.ThirstThresholds[ThirstThreshold.Okay] - 1);
			((EntitySystem)this).DirtyField<ThirstComponent>(uid, component, "CurrentThirst", (MetaDataComponent)null);
		}
		component.NextUpdateTime = _timing.CurTime;
		component.CurrentThirstThreshold = GetThirstThreshold(component, component.CurrentThirst);
		component.LastThirstThreshold = ThirstThreshold.Okay;
		UpdateEffects(uid, component);
		((EntitySystem)this).DirtyFields<ThirstComponent>(uid, component, (MetaDataComponent)null, new string[3] { "NextUpdateTime", "CurrentThirstThreshold", "LastThirstThreshold" });
		MovementSpeedModifierComponent moveMod = default(MovementSpeedModifierComponent);
		((EntitySystem)this).TryComp<MovementSpeedModifierComponent>(uid, ref moveMod);
		_movement.RefreshMovementSpeedModifiers(uid, moveMod);
	}

	private void OnRefreshMovespeed(EntityUid uid, ThirstComponent component, RefreshMovementSpeedModifiersEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!DisableThirstMovespeedPenalty && !_jetpack.IsUserFlying(uid))
		{
			float mod = (((int)component.CurrentThirstThreshold <= 1) ? 0.75f : 1f);
			args.ModifySpeed(mod, mod);
		}
	}

	private void OnRejuvenate(EntityUid uid, ThirstComponent component, RejuvenateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SetThirst(uid, component, component.ThirstThresholds[ThirstThreshold.Okay]);
	}

	private ThirstThreshold GetThirstThreshold(ThirstComponent component, float amount)
	{
		ThirstThreshold result = ThirstThreshold.Dead;
		float value = component.ThirstThresholds[ThirstThreshold.OverHydrated];
		foreach (KeyValuePair<ThirstThreshold, float> threshold in component.ThirstThresholds)
		{
			if (threshold.Value <= value && threshold.Value >= amount)
			{
				result = threshold.Key;
				value = threshold.Value;
			}
		}
		return result;
	}

	public void ModifyThirst(EntityUid uid, ThirstComponent component, float amount)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SetThirst(uid, component, component.CurrentThirst + amount);
	}

	public void SetThirst(EntityUid uid, ThirstComponent component, float amount)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		component.CurrentThirst = Math.Clamp(amount, component.ThirstThresholds[ThirstThreshold.Dead], component.ThirstThresholds[ThirstThreshold.OverHydrated]);
		((EntitySystem)this).DirtyField<ThirstComponent>(uid, component, "CurrentThirst", (MetaDataComponent)null);
	}

	private bool IsMovementThreshold(ThirstThreshold threshold)
	{
		switch (threshold)
		{
		case ThirstThreshold.Dead:
		case ThirstThreshold.Parched:
			return true;
		case ThirstThreshold.Thirsty:
		case ThirstThreshold.Okay:
		case ThirstThreshold.OverHydrated:
			return false;
		default:
			throw new ArgumentOutOfRangeException("threshold", threshold, null);
		}
	}

	public bool TryGetStatusIconPrototype(ThirstComponent component, [NotNullWhen(true)] out SatiationIconPrototype? prototype)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		switch (component.CurrentThirstThreshold)
		{
		case ThirstThreshold.OverHydrated:
			_prototype.TryIndex<SatiationIconPrototype>(ThirstIconOverhydratedId, ref prototype);
			break;
		case ThirstThreshold.Thirsty:
			_prototype.TryIndex<SatiationIconPrototype>(ThirstIconThirstyId, ref prototype);
			break;
		case ThirstThreshold.Parched:
			_prototype.TryIndex<SatiationIconPrototype>(ThirstIconParchedId, ref prototype);
			break;
		default:
			prototype = null;
			break;
		}
		return prototype != null;
	}

	private void UpdateEffects(EntityUid uid, ThirstComponent component)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		MovementSpeedModifierComponent movementSlowdownComponent = default(MovementSpeedModifierComponent);
		if (IsMovementThreshold(component.LastThirstThreshold) != IsMovementThreshold(component.CurrentThirstThreshold) && ((EntitySystem)this).TryComp<MovementSpeedModifierComponent>(uid, ref movementSlowdownComponent))
		{
			_movement.RefreshMovementSpeedModifiers(uid, movementSlowdownComponent);
		}
		if (ThirstComponent.ThirstThresholdAlertTypes.TryGetValue(component.CurrentThirstThreshold, out ProtoId<AlertPrototype> alertId))
		{
			_alerts.ShowAlert(uid, alertId);
		}
		else
		{
			_alerts.ClearAlertCategory(uid, component.ThirstyCategory);
		}
		((EntitySystem)this).DirtyField<ThirstComponent>(uid, component, "LastThirstThreshold", (MetaDataComponent)null);
		((EntitySystem)this).DirtyField<ThirstComponent>(uid, component, "ActualDecayRate", (MetaDataComponent)null);
		switch (component.CurrentThirstThreshold)
		{
		case ThirstThreshold.OverHydrated:
			component.LastThirstThreshold = component.CurrentThirstThreshold;
			component.ActualDecayRate = component.BaseDecayRate * 1.2f;
			break;
		case ThirstThreshold.Okay:
			component.LastThirstThreshold = component.CurrentThirstThreshold;
			component.ActualDecayRate = component.BaseDecayRate;
			break;
		case ThirstThreshold.Thirsty:
			component.LastThirstThreshold = component.CurrentThirstThreshold;
			component.ActualDecayRate = component.BaseDecayRate * 0.8f;
			break;
		case ThirstThreshold.Parched:
			_movement.RefreshMovementSpeedModifiers(uid);
			component.LastThirstThreshold = component.CurrentThirstThreshold;
			component.ActualDecayRate = component.BaseDecayRate * 0.6f;
			break;
		case ThirstThreshold.Dead:
			break;
		default:
			((EntitySystem)this).Log.Error($"No thirst threshold found for {component.CurrentThirstThreshold}");
			throw new ArgumentOutOfRangeException($"No thirst threshold found for {component.CurrentThirstThreshold}");
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<ThirstComponent> query = ((EntitySystem)this).EntityQueryEnumerator<ThirstComponent>();
		EntityUid uid = default(EntityUid);
		ThirstComponent thirst = default(ThirstComponent);
		while (query.MoveNext(ref uid, ref thirst))
		{
			if (!(_timing.CurTime < thirst.NextUpdateTime))
			{
				thirst.NextUpdateTime += thirst.UpdateRate;
				ModifyThirst(uid, thirst, 0f - thirst.ActualDecayRate);
				ThirstThreshold calculatedThirstThreshold = GetThirstThreshold(thirst, thirst.CurrentThirst);
				if (calculatedThirstThreshold != thirst.CurrentThirstThreshold)
				{
					thirst.CurrentThirstThreshold = calculatedThirstThreshold;
					UpdateEffects(uid, thirst);
				}
			}
		}
	}
}
