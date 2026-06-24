using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.TrainingDummy;

[RegisterComponent]
public sealed class RMCTrainingDummyComponent : Component, ISerializationGenerated<RMCTrainingDummyComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public ComponentRegistry? RemoveComponents;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCTrainingDummyComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RMCTrainingDummyComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RMCTrainingDummyComponent>(this, ref target, hookCtx, false, context))
		{
			ComponentRegistry RemoveComponentsTemp = null;
			if (!serialization.TryCustomCopy<ComponentRegistry>(RemoveComponents, ref RemoveComponentsTemp, hookCtx, false, context))
			{
				RemoveComponentsTemp = serialization.CreateCopy<ComponentRegistry>(RemoveComponents, hookCtx, context, false);
			}
			target.RemoveComponents = RemoveComponentsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCTrainingDummyComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCTrainingDummyComponent cast = (RMCTrainingDummyComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCTrainingDummyComponent cast = (RMCTrainingDummyComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCTrainingDummyComponent def = (RMCTrainingDummyComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RMCTrainingDummyComponent Instantiate()
	{
		return new RMCTrainingDummyComponent();
	}
}
