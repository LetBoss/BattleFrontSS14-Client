using System;
using System.Collections.Generic;
using Content.Shared.Research.Prototypes;
using Content.Shared.Research.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Research.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(BlueprintSystem) })]
public sealed class BlueprintComponent : Component, ISerializationGenerated<BlueprintComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public HashSet<ProtoId<LatheRecipePrototype>> ProvidedRecipes = new HashSet<ProtoId<LatheRecipePrototype>>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref BlueprintComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (BlueprintComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<BlueprintComponent>(this, ref target, hookCtx, false, context))
		{
			HashSet<ProtoId<LatheRecipePrototype>> ProvidedRecipesTemp = null;
			if (ProvidedRecipes == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<ProtoId<LatheRecipePrototype>>>(ProvidedRecipes, ref ProvidedRecipesTemp, hookCtx, true, context))
			{
				ProvidedRecipesTemp = serialization.CreateCopy<HashSet<ProtoId<LatheRecipePrototype>>>(ProvidedRecipes, hookCtx, context, false);
			}
			target.ProvidedRecipes = ProvidedRecipesTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref BlueprintComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BlueprintComponent cast = (BlueprintComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BlueprintComponent cast = (BlueprintComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BlueprintComponent def = (BlueprintComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override BlueprintComponent Instantiate()
	{
		return new BlueprintComponent();
	}
}
