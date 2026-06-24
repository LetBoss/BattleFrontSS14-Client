using System;
using System.Collections.Generic;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Humanoid.Prototypes;
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
public sealed class RMCRandomMarkingsComponent : Component, ISerializationGenerated<RMCRandomMarkingsComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public Dictionary<ProtoId<SpeciesPrototype>, Dictionary<MarkingCategories, float>> Markings;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCRandomMarkingsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RMCRandomMarkingsComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RMCRandomMarkingsComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<ProtoId<SpeciesPrototype>, Dictionary<MarkingCategories, float>> MarkingsTemp = null;
			if (Markings == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<ProtoId<SpeciesPrototype>, Dictionary<MarkingCategories, float>>>(Markings, ref MarkingsTemp, hookCtx, true, context))
			{
				MarkingsTemp = serialization.CreateCopy<Dictionary<ProtoId<SpeciesPrototype>, Dictionary<MarkingCategories, float>>>(Markings, hookCtx, context, false);
			}
			target.Markings = MarkingsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCRandomMarkingsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCRandomMarkingsComponent cast = (RMCRandomMarkingsComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCRandomMarkingsComponent cast = (RMCRandomMarkingsComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCRandomMarkingsComponent def = (RMCRandomMarkingsComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RMCRandomMarkingsComponent Instantiate()
	{
		return new RMCRandomMarkingsComponent();
	}
}
