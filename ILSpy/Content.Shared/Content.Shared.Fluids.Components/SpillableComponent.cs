using System;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Fluids.Components;

[RegisterComponent]
public sealed class SpillableComponent : Component, ISerializationGenerated<SpillableComponent>, ISerializationGenerated
{
	[DataField("solution", false, 1, false, false, null)]
	public string SolutionName = "puddle";

	[DataField(null, false, 1, false, false, null)]
	public float? SpillDelay;

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 MaxMeleeSpillAmount = FixedPoint2.New(20);

	[DataField(null, false, 1, false, false, null)]
	public bool SpillWhenThrown = true;

	[DataField(null, false, 1, false, false, null)]
	public bool PreventMelee = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SpillableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SpillableComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<SpillableComponent>(this, ref target, hookCtx, false, context))
		{
			string SolutionNameTemp = null;
			if (SolutionName == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(SolutionName, ref SolutionNameTemp, hookCtx, false, context))
			{
				SolutionNameTemp = SolutionName;
			}
			target.SolutionName = SolutionNameTemp;
			float? SpillDelayTemp = null;
			if (!serialization.TryCustomCopy<float?>(SpillDelay, ref SpillDelayTemp, hookCtx, false, context))
			{
				SpillDelayTemp = SpillDelay;
			}
			target.SpillDelay = SpillDelayTemp;
			FixedPoint2 MaxMeleeSpillAmountTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(MaxMeleeSpillAmount, ref MaxMeleeSpillAmountTemp, hookCtx, false, context))
			{
				MaxMeleeSpillAmountTemp = serialization.CreateCopy<FixedPoint2>(MaxMeleeSpillAmount, hookCtx, context, false);
			}
			target.MaxMeleeSpillAmount = MaxMeleeSpillAmountTemp;
			bool SpillWhenThrownTemp = false;
			if (!serialization.TryCustomCopy<bool>(SpillWhenThrown, ref SpillWhenThrownTemp, hookCtx, false, context))
			{
				SpillWhenThrownTemp = SpillWhenThrown;
			}
			target.SpillWhenThrown = SpillWhenThrownTemp;
			bool PreventMeleeTemp = false;
			if (!serialization.TryCustomCopy<bool>(PreventMelee, ref PreventMeleeTemp, hookCtx, false, context))
			{
				PreventMeleeTemp = PreventMelee;
			}
			target.PreventMelee = PreventMeleeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SpillableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SpillableComponent cast = (SpillableComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SpillableComponent cast = (SpillableComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SpillableComponent def = (SpillableComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SpillableComponent Instantiate()
	{
		return new SpillableComponent();
	}
}
