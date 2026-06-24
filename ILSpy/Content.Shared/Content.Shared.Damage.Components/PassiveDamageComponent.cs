using System;
using System.Collections.Generic;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs;
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
public sealed class PassiveDamageComponent : Component, ISerializationGenerated<PassiveDamageComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public List<MobState> AllowedStates = new List<MobState>();

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public DamageSpecifier Damage = new DamageSpecifier();

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float Interval = 1f;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public FixedPoint2 DamageCap = 0;

	[DataField("nextDamage", false, 1, false, false, typeof(TimeOffsetSerializer))]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public TimeSpan NextDamage = TimeSpan.Zero;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PassiveDamageComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PassiveDamageComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<PassiveDamageComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		List<MobState> AllowedStatesTemp = null;
		if (AllowedStates == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<List<MobState>>(AllowedStates, ref AllowedStatesTemp, hookCtx, true, context))
		{
			AllowedStatesTemp = serialization.CreateCopy<List<MobState>>(AllowedStates, hookCtx, context, false);
		}
		target.AllowedStates = AllowedStatesTemp;
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
		float IntervalTemp = 0f;
		if (!serialization.TryCustomCopy<float>(Interval, ref IntervalTemp, hookCtx, false, context))
		{
			IntervalTemp = Interval;
		}
		target.Interval = IntervalTemp;
		FixedPoint2 DamageCapTemp = default(FixedPoint2);
		if (!serialization.TryCustomCopy<FixedPoint2>(DamageCap, ref DamageCapTemp, hookCtx, false, context))
		{
			DamageCapTemp = serialization.CreateCopy<FixedPoint2>(DamageCap, hookCtx, context, false);
		}
		target.DamageCap = DamageCapTemp;
		TimeSpan NextDamageTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(NextDamage, ref NextDamageTemp, hookCtx, false, context))
		{
			NextDamageTemp = serialization.CreateCopy<TimeSpan>(NextDamage, hookCtx, context, false);
		}
		target.NextDamage = NextDamageTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PassiveDamageComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PassiveDamageComponent cast = (PassiveDamageComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PassiveDamageComponent cast = (PassiveDamageComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PassiveDamageComponent def = (PassiveDamageComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PassiveDamageComponent Instantiate()
	{
		return new PassiveDamageComponent();
	}
}
