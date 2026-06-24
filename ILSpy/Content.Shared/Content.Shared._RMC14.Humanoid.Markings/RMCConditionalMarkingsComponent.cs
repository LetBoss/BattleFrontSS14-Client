using System;
using System.Collections.Generic;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Humanoid.Markings;

[RegisterComponent]
[NetworkedComponent]
public sealed class RMCConditionalMarkingsComponent : Component, ISerializationGenerated<RMCConditionalMarkingsComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public Dictionary<Sex, List<ProtoId<MarkingPrototype>>> Markings;

	[DataField(null, false, 1, false, false, null)]
	public MarkingCategories TargetCategory;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCConditionalMarkingsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RMCConditionalMarkingsComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RMCConditionalMarkingsComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<Sex, List<ProtoId<MarkingPrototype>>> MarkingsTemp = null;
			if (Markings == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<Sex, List<ProtoId<MarkingPrototype>>>>(Markings, ref MarkingsTemp, hookCtx, true, context))
			{
				MarkingsTemp = serialization.CreateCopy<Dictionary<Sex, List<ProtoId<MarkingPrototype>>>>(Markings, hookCtx, context, false);
			}
			target.Markings = MarkingsTemp;
			MarkingCategories TargetCategoryTemp = MarkingCategories.Special;
			if (!serialization.TryCustomCopy<MarkingCategories>(TargetCategory, ref TargetCategoryTemp, hookCtx, false, context))
			{
				TargetCategoryTemp = TargetCategory;
			}
			target.TargetCategory = TargetCategoryTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCConditionalMarkingsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCConditionalMarkingsComponent cast = (RMCConditionalMarkingsComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCConditionalMarkingsComponent cast = (RMCConditionalMarkingsComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCConditionalMarkingsComponent def = (RMCConditionalMarkingsComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RMCConditionalMarkingsComponent Instantiate()
	{
		return new RMCConditionalMarkingsComponent();
	}
}
