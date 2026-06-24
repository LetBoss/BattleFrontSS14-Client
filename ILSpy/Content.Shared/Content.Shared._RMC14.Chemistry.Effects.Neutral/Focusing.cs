using System;
using Content.Shared._RMC14.Body;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Drunk;
using Content.Shared.EntityEffects;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Eye.Blinding.Systems;
using Content.Shared.FixedPoint;
using Content.Shared.Speech.EntitySystems;
using Content.Shared.Speech.Muting;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Chemistry.Effects.Neutral;

public sealed class Focusing : RMCChemicalEffect, ISerializationGenerated<Focusing>, ISerializationGenerated
{
	private static readonly ProtoId<DamageTypePrototype> PoisonType = ProtoId<DamageTypePrototype>.op_Implicit("Poison");

	protected override string ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		string focusing = ((base.ActualPotency >= 3f) ? ". Also powerful enough to instantly cure mute and blindness." : ".");
		return $"Removes [color=green]{base.PotencyPerSecond}[/color] units of alcoholic substances and [color=green]{base.PotencyPerSecond * 2f}[/color] seconds of drunkenness{focusing}\nOverdoses cause [color=red]{base.PotencyPerSecond}[/color] toxin damage.\nCritical overdoses cause [color=red]{base.PotencyPerSecond * 3f}[/color] toxin damage";
	}

	protected override void Tick(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		SharedRMCBloodstreamSystem sharedRMCBloodstreamSystem = args.EntityManager.System<SharedRMCBloodstreamSystem>();
		SharedDrunkSystem drunkSystem = args.EntityManager.System<SharedDrunkSystem>();
		SharedStutteringSystem stutterSystem = args.EntityManager.System<SharedStutteringSystem>();
		StatusEffectsSystem statusEffects = args.EntityManager.System<StatusEffectsSystem>();
		sharedRMCBloodstreamSystem.RemoveBloodstreamAlcohols(args.TargetEntity, potency);
		drunkSystem.TryRemoveDrunkenessTime(args.TargetEntity, base.PotencyPerSecond * 2f);
		stutterSystem.DoRemoveStutterTime(args.TargetEntity, base.PotencyPerSecond * 2f);
		statusEffects.TryRemoveTime(args.TargetEntity, "Jitter", TimeSpan.FromSeconds(base.PotencyPerSecond * 2f));
		if (base.ActualPotency >= 3f)
		{
			args.EntityManager.EntitySysManager.GetEntitySystem<BlindableSystem>().AdjustEyeDamage(Entity<BlindableComponent>.op_Implicit(args.TargetEntity), -9);
			args.EntityManager.RemoveComponent<MutedComponent>(args.TargetEntity, (MetaDataComponent)null);
		}
	}

	protected override void TickOverdose(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		DamageSpecifier damage = new DamageSpecifier();
		damage.DamageDict[ProtoId<DamageTypePrototype>.op_Implicit(PoisonType)] = potency;
		damageable.TryChangeDamage(args.TargetEntity, damage, ignoreResistances: true, interruptsDoAfters: false);
	}

	protected override void TickCriticalOverdose(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		DamageSpecifier damage = new DamageSpecifier();
		damage.DamageDict[ProtoId<DamageTypePrototype>.op_Implicit(PoisonType)] = potency * 3;
		damageable.TryChangeDamage(args.TargetEntity, damage, ignoreResistances: true, interruptsDoAfters: false);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref Focusing target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCChemicalEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (Focusing)definitionCast;
		serialization.TryCustomCopy<Focusing>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref Focusing target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref RMCChemicalEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Focusing cast = (Focusing)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Focusing cast = (Focusing)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override Focusing Instantiate()
	{
		return new Focusing();
	}
}
