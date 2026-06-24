using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Client.Storage.Visualizers;

[RegisterComponent]
[Access(new Type[] { typeof(EntityStorageVisualizerSystem) })]
public sealed class EntityStorageVisualsComponent : Component, ISerializationGenerated<EntityStorageVisualsComponent>, ISerializationGenerated
{
	[DataField("stateBaseClosed", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string? StateBaseClosed;

	[DataField("stateBaseOpen", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string? StateBaseOpen;

	[DataField("stateDoorOpen", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string? StateDoorOpen;

	[DataField("stateDoorClosed", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string? StateDoorClosed;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public int? OpenDrawDepth;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public int? ClosedDrawDepth;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref EntityStorageVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (EntityStorageVisualsComponent)(object)val;
		if (!serialization.TryCustomCopy<EntityStorageVisualsComponent>(this, ref target, hookCtx, false, context))
		{
			string stateBaseClosed = null;
			if (!serialization.TryCustomCopy<string>(StateBaseClosed, ref stateBaseClosed, hookCtx, false, context))
			{
				stateBaseClosed = StateBaseClosed;
			}
			target.StateBaseClosed = stateBaseClosed;
			string stateBaseOpen = null;
			if (!serialization.TryCustomCopy<string>(StateBaseOpen, ref stateBaseOpen, hookCtx, false, context))
			{
				stateBaseOpen = StateBaseOpen;
			}
			target.StateBaseOpen = stateBaseOpen;
			string stateDoorOpen = null;
			if (!serialization.TryCustomCopy<string>(StateDoorOpen, ref stateDoorOpen, hookCtx, false, context))
			{
				stateDoorOpen = StateDoorOpen;
			}
			target.StateDoorOpen = stateDoorOpen;
			string stateDoorClosed = null;
			if (!serialization.TryCustomCopy<string>(StateDoorClosed, ref stateDoorClosed, hookCtx, false, context))
			{
				stateDoorClosed = StateDoorClosed;
			}
			target.StateDoorClosed = stateDoorClosed;
			int? openDrawDepth = null;
			if (!serialization.TryCustomCopy<int?>(OpenDrawDepth, ref openDrawDepth, hookCtx, false, context))
			{
				openDrawDepth = OpenDrawDepth;
			}
			target.OpenDrawDepth = openDrawDepth;
			int? closedDrawDepth = null;
			if (!serialization.TryCustomCopy<int?>(ClosedDrawDepth, ref closedDrawDepth, hookCtx, false, context))
			{
				closedDrawDepth = ClosedDrawDepth;
			}
			target.ClosedDrawDepth = closedDrawDepth;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref EntityStorageVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityStorageVisualsComponent target2 = (EntityStorageVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityStorageVisualsComponent target2 = (EntityStorageVisualsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityStorageVisualsComponent target2 = (EntityStorageVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override EntityStorageVisualsComponent Instantiate()
	{
		return new EntityStorageVisualsComponent();
	}
}
