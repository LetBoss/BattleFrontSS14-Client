using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Containers;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(DragInsertContainerSystem) })]
public sealed class DragInsertContainerComponent : Component, ISerializationGenerated<DragInsertContainerComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string ContainerId;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool UseVerbs = true;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan EntryDelay = TimeSpan.Zero;

	[DataField(null, false, 1, false, false, null)]
	public bool DelaySelfEntry;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DragInsertContainerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (DragInsertContainerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<DragInsertContainerComponent>(this, ref target, hookCtx, false, context))
		{
			string ContainerIdTemp = null;
			if (ContainerId == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(ContainerId, ref ContainerIdTemp, hookCtx, false, context))
			{
				ContainerIdTemp = ContainerId;
			}
			target.ContainerId = ContainerIdTemp;
			bool UseVerbsTemp = false;
			if (!serialization.TryCustomCopy<bool>(UseVerbs, ref UseVerbsTemp, hookCtx, false, context))
			{
				UseVerbsTemp = UseVerbs;
			}
			target.UseVerbs = UseVerbsTemp;
			TimeSpan EntryDelayTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(EntryDelay, ref EntryDelayTemp, hookCtx, false, context))
			{
				EntryDelayTemp = serialization.CreateCopy<TimeSpan>(EntryDelay, hookCtx, context, false);
			}
			target.EntryDelay = EntryDelayTemp;
			bool DelaySelfEntryTemp = false;
			if (!serialization.TryCustomCopy<bool>(DelaySelfEntry, ref DelaySelfEntryTemp, hookCtx, false, context))
			{
				DelaySelfEntryTemp = DelaySelfEntry;
			}
			target.DelaySelfEntry = DelaySelfEntryTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DragInsertContainerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DragInsertContainerComponent cast = (DragInsertContainerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DragInsertContainerComponent cast = (DragInsertContainerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DragInsertContainerComponent def = (DragInsertContainerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override DragInsertContainerComponent Instantiate()
	{
		return new DragInsertContainerComponent();
	}
}
