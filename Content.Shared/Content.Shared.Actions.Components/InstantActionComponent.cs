using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Actions.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedActionsSystem) })]
[EntityCategory(new string[] { "Actions" })]
public sealed class InstantActionComponent : Component, ISerializationGenerated<InstantActionComponent>, ISerializationGenerated
{
	[NonSerialized]
	[DataField(null, false, 1, true, false, null)]
	public InstantActionEvent? Event;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref InstantActionComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (InstantActionComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<InstantActionComponent>(this, ref target, hookCtx, false, context))
		{
			InstantActionEvent EventTemp = null;
			if (!serialization.TryCustomCopy<InstantActionEvent>(Event, ref EventTemp, hookCtx, true, context))
			{
				EventTemp = serialization.CreateCopy<InstantActionEvent>(Event, hookCtx, context, false);
			}
			target.Event = EventTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref InstantActionComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InstantActionComponent cast = (InstantActionComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InstantActionComponent cast = (InstantActionComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InstantActionComponent def = (InstantActionComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override InstantActionComponent Instantiate()
	{
		return new InstantActionComponent();
	}
}
