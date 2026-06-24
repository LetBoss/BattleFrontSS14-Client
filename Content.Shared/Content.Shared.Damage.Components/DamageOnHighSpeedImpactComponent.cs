using System;
using Content.Shared.Damage.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Damage.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(DamageOnHighSpeedImpactSystem) })]
public sealed class DamageOnHighSpeedImpactComponent : Component, ISerializationGenerated<DamageOnHighSpeedImpactComponent>, ISerializationGenerated
{
	[DataField("minimumSpeed", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float MinimumSpeed = 20f;

	[DataField("speedDamageFactor", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float SpeedDamageFactor = 0.5f;

	[DataField("soundHit", false, 1, true, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public SoundSpecifier SoundHit;

	[DataField("stunChance", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float StunChance = 0.25f;

	[DataField("stunMinimumDamage", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public int StunMinimumDamage = 10;

	[DataField("stunSeconds", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float StunSeconds = 1f;

	[DataField("damageCooldown", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float DamageCooldown = 2f;

	[DataField("lastHit", false, 1, false, false, typeof(TimeOffsetSerializer))]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public TimeSpan? LastHit;

	[DataField("damage", false, 1, true, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public DamageSpecifier Damage;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DamageOnHighSpeedImpactComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (DamageOnHighSpeedImpactComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<DamageOnHighSpeedImpactComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		float MinimumSpeedTemp = 0f;
		if (!serialization.TryCustomCopy<float>(MinimumSpeed, ref MinimumSpeedTemp, hookCtx, false, context))
		{
			MinimumSpeedTemp = MinimumSpeed;
		}
		target.MinimumSpeed = MinimumSpeedTemp;
		float SpeedDamageFactorTemp = 0f;
		if (!serialization.TryCustomCopy<float>(SpeedDamageFactor, ref SpeedDamageFactorTemp, hookCtx, false, context))
		{
			SpeedDamageFactorTemp = SpeedDamageFactor;
		}
		target.SpeedDamageFactor = SpeedDamageFactorTemp;
		SoundSpecifier SoundHitTemp = null;
		if (SoundHit == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<SoundSpecifier>(SoundHit, ref SoundHitTemp, hookCtx, true, context))
		{
			SoundHitTemp = serialization.CreateCopy<SoundSpecifier>(SoundHit, hookCtx, context, false);
		}
		target.SoundHit = SoundHitTemp;
		float StunChanceTemp = 0f;
		if (!serialization.TryCustomCopy<float>(StunChance, ref StunChanceTemp, hookCtx, false, context))
		{
			StunChanceTemp = StunChance;
		}
		target.StunChance = StunChanceTemp;
		int StunMinimumDamageTemp = 0;
		if (!serialization.TryCustomCopy<int>(StunMinimumDamage, ref StunMinimumDamageTemp, hookCtx, false, context))
		{
			StunMinimumDamageTemp = StunMinimumDamage;
		}
		target.StunMinimumDamage = StunMinimumDamageTemp;
		float StunSecondsTemp = 0f;
		if (!serialization.TryCustomCopy<float>(StunSeconds, ref StunSecondsTemp, hookCtx, false, context))
		{
			StunSecondsTemp = StunSeconds;
		}
		target.StunSeconds = StunSecondsTemp;
		float DamageCooldownTemp = 0f;
		if (!serialization.TryCustomCopy<float>(DamageCooldown, ref DamageCooldownTemp, hookCtx, false, context))
		{
			DamageCooldownTemp = DamageCooldown;
		}
		target.DamageCooldown = DamageCooldownTemp;
		TimeSpan? LastHitTemp = null;
		if (!serialization.TryCustomCopy<TimeSpan?>(LastHit, ref LastHitTemp, hookCtx, false, context))
		{
			LastHitTemp = serialization.CreateCopy<TimeSpan?>(LastHit, hookCtx, context, false);
		}
		target.LastHit = LastHitTemp;
		DamageSpecifier DamageTemp = null;
		if (Damage == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<DamageSpecifier>(Damage, ref DamageTemp, hookCtx, false, context))
		{
			if (Damage == null)
			{
				DamageTemp = null;
			}
			else
			{
				serialization.CopyTo<DamageSpecifier>(Damage, ref DamageTemp, hookCtx, context, true);
			}
		}
		target.Damage = DamageTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DamageOnHighSpeedImpactComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamageOnHighSpeedImpactComponent cast = (DamageOnHighSpeedImpactComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamageOnHighSpeedImpactComponent cast = (DamageOnHighSpeedImpactComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamageOnHighSpeedImpactComponent def = (DamageOnHighSpeedImpactComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override DamageOnHighSpeedImpactComponent Instantiate()
	{
		return new DamageOnHighSpeedImpactComponent();
	}
}
