using System;
using System.Collections.Generic;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Zombies;

[RegisterComponent]
[NetworkedComponent]
public sealed class PendingZombieComponent : Component, ISerializationGenerated<PendingZombieComponent>, ISerializationGenerated
{
	[DataField("damage", false, 1, false, false, null)]
	public DamageSpecifier Damage = new DamageSpecifier
	{
		DamageDict = new Dictionary<string, FixedPoint2> { { "Poison", 0.4 } }
	};

	[DataField("critDamageMultiplier", false, 1, false, false, null)]
	public float CritDamageMultiplier = 10f;

	[DataField("nextTick", false, 1, false, false, typeof(TimeOffsetSerializer))]
	public TimeSpan NextTick;

	[DataField("gracePeriod", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public TimeSpan GracePeriod = TimeSpan.Zero;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan MinInitialInfectedGrace = TimeSpan.FromMinutes(12.5);

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan MaxInitialInfectedGrace = TimeSpan.FromMinutes(15.0);

	[DataField("infectionWarningChance", false, 1, false, false, null)]
	public float InfectionWarningChance = 0.0166f;

	[DataField("infectionWarnings", false, 1, false, false, null)]
	public List<string> InfectionWarnings = new List<string> { "zombie-infection-warning", "zombie-infection-underway" };

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PendingZombieComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PendingZombieComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<PendingZombieComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
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
		float CritDamageMultiplierTemp = 0f;
		if (!serialization.TryCustomCopy<float>(CritDamageMultiplier, ref CritDamageMultiplierTemp, hookCtx, false, context))
		{
			CritDamageMultiplierTemp = CritDamageMultiplier;
		}
		target.CritDamageMultiplier = CritDamageMultiplierTemp;
		TimeSpan NextTickTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(NextTick, ref NextTickTemp, hookCtx, false, context))
		{
			NextTickTemp = serialization.CreateCopy<TimeSpan>(NextTick, hookCtx, context, false);
		}
		target.NextTick = NextTickTemp;
		TimeSpan GracePeriodTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(GracePeriod, ref GracePeriodTemp, hookCtx, false, context))
		{
			GracePeriodTemp = serialization.CreateCopy<TimeSpan>(GracePeriod, hookCtx, context, false);
		}
		target.GracePeriod = GracePeriodTemp;
		TimeSpan MinInitialInfectedGraceTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(MinInitialInfectedGrace, ref MinInitialInfectedGraceTemp, hookCtx, false, context))
		{
			MinInitialInfectedGraceTemp = serialization.CreateCopy<TimeSpan>(MinInitialInfectedGrace, hookCtx, context, false);
		}
		target.MinInitialInfectedGrace = MinInitialInfectedGraceTemp;
		TimeSpan MaxInitialInfectedGraceTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(MaxInitialInfectedGrace, ref MaxInitialInfectedGraceTemp, hookCtx, false, context))
		{
			MaxInitialInfectedGraceTemp = serialization.CreateCopy<TimeSpan>(MaxInitialInfectedGrace, hookCtx, context, false);
		}
		target.MaxInitialInfectedGrace = MaxInitialInfectedGraceTemp;
		float InfectionWarningChanceTemp = 0f;
		if (!serialization.TryCustomCopy<float>(InfectionWarningChance, ref InfectionWarningChanceTemp, hookCtx, false, context))
		{
			InfectionWarningChanceTemp = InfectionWarningChance;
		}
		target.InfectionWarningChance = InfectionWarningChanceTemp;
		List<string> InfectionWarningsTemp = null;
		if (InfectionWarnings == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<List<string>>(InfectionWarnings, ref InfectionWarningsTemp, hookCtx, true, context))
		{
			InfectionWarningsTemp = serialization.CreateCopy<List<string>>(InfectionWarnings, hookCtx, context, false);
		}
		target.InfectionWarnings = InfectionWarningsTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PendingZombieComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PendingZombieComponent cast = (PendingZombieComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PendingZombieComponent cast = (PendingZombieComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PendingZombieComponent def = (PendingZombieComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PendingZombieComponent Instantiate()
	{
		return new PendingZombieComponent();
	}
}
