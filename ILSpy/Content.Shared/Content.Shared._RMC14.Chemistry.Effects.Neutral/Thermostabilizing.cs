using System;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Temperature;
using Content.Shared.Damage;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Content.Shared.StatusEffect;
using Content.Shared.Temperature;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Chemistry.Effects.Neutral;

public sealed class Thermostabilizing : RMCChemicalEffect, ISerializationGenerated<Thermostabilizing>, ISerializationGenerated
{
	private static readonly ProtoId<StatusEffectPrototype> Unconscious = ProtoId<StatusEffectPrototype>.op_Implicit("Unconscious");

	protected override string ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return $"Stabilizes the temperature of the body to [color=green]{TemperatureHelpers.CelsiusToKelvin(37f)}[/color] kelvins, by [color=green]{40f * base.PotencyPerSecond * 1.5f}[/color] K at a time.\nOverdoses cause [color=red]10[/color] seconds of unconsciousness.\nCritical overdoses cause [color=red]5[/color] seconds of unconsciousness with a [color=red]5%[/color] chance";
	}

	protected override void Tick(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		SharedRMCTemperatureSystem sys = args.EntityManager.EntitySysManager.GetEntitySystem<SharedRMCTemperatureSystem>();
		float current = sys.GetTemperature(args.TargetEntity);
		float normalBodyTemp = TemperatureHelpers.CelsiusToKelvin(37f);
		if (!((double)Math.Abs(current - normalBodyTemp) < 0.01))
		{
			float change = 40f * potency.Float() * 1.5f;
			float temp = ((current > normalBodyTemp) ? Math.Max(normalBodyTemp, current - change) : Math.Min(normalBodyTemp, current + change));
			sys.ForceChangeTemperature(args.TargetEntity, temp);
		}
	}

	protected override void TickOverdose(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		args.EntityManager.System<StatusEffectsSystem>().TryAddStatusEffect<RMCUnconsciousComponent>(args.TargetEntity, ProtoId<StatusEffectPrototype>.op_Implicit(Unconscious), TimeSpan.FromSeconds(10L), false, (StatusEffectsComponent?)null, false);
	}

	protected override void TickCriticalOverdose(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (RandomExtensions.Prob(IoCManager.Resolve<IRobustRandom>(), 0.05f))
		{
			args.EntityManager.System<StatusEffectsSystem>().TryAddStatusEffect<RMCUnconsciousComponent>(args.TargetEntity, ProtoId<StatusEffectPrototype>.op_Implicit(Unconscious), TimeSpan.FromSeconds(5L), false, (StatusEffectsComponent?)null, false);
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref Thermostabilizing target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCChemicalEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (Thermostabilizing)definitionCast;
		serialization.TryCustomCopy<Thermostabilizing>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref Thermostabilizing target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref RMCChemicalEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Thermostabilizing cast = (Thermostabilizing)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Thermostabilizing cast = (Thermostabilizing)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override Thermostabilizing Instantiate()
	{
		return new Thermostabilizing();
	}
}
