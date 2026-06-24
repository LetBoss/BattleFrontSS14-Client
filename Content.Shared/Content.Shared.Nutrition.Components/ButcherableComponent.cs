using System;
using System.Collections.Generic;
using Content.Shared.Storage;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Nutrition.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class ButcherableComponent : Component, ISerializationGenerated<ButcherableComponent>, ISerializationGenerated
{
	[DataField("spawned", false, 1, true, false, null)]
	public List<EntitySpawnEntry> SpawnedEntities = new List<EntitySpawnEntry>();

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("butcherDelay", false, 1, false, false, null)]
	public float ButcherDelay = 8f;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("butcheringType", false, 1, false, false, null)]
	public ButcheringType Type;

	[ViewVariables]
	public bool BeingButchered;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ButcherableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ButcherableComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ButcherableComponent>(this, ref target, hookCtx, false, context))
		{
			List<EntitySpawnEntry> SpawnedEntitiesTemp = null;
			if (SpawnedEntities == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<EntitySpawnEntry>>(SpawnedEntities, ref SpawnedEntitiesTemp, hookCtx, true, context))
			{
				SpawnedEntitiesTemp = serialization.CreateCopy<List<EntitySpawnEntry>>(SpawnedEntities, hookCtx, context, false);
			}
			target.SpawnedEntities = SpawnedEntitiesTemp;
			float ButcherDelayTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ButcherDelay, ref ButcherDelayTemp, hookCtx, false, context))
			{
				ButcherDelayTemp = ButcherDelay;
			}
			target.ButcherDelay = ButcherDelayTemp;
			ButcheringType TypeTemp = ButcheringType.Knife;
			if (!serialization.TryCustomCopy<ButcheringType>(Type, ref TypeTemp, hookCtx, false, context))
			{
				TypeTemp = Type;
			}
			target.Type = TypeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ButcherableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ButcherableComponent cast = (ButcherableComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ButcherableComponent cast = (ButcherableComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ButcherableComponent def = (ButcherableComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ButcherableComponent Instantiate()
	{
		return new ButcherableComponent();
	}
}
