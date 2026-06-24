using System;
using Content.Shared.Actions;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations;

namespace Content.Shared.UserInterface;

public sealed class ToggleIntrinsicUIEvent : InstantActionEvent, ISerializationGenerated<ToggleIntrinsicUIEvent>, ISerializationGenerated
{
	[DataField("key", false, 1, true, false, typeof(EnumSerializer))]
	public Enum? Key { get; set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ToggleIntrinsicUIEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InstantActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ToggleIntrinsicUIEvent)definitionCast;
		if (!serialization.TryCustomCopy<ToggleIntrinsicUIEvent>(this, ref target, hookCtx, false, context))
		{
			Enum KeyTemp = null;
			if (!serialization.TryCustomCopy<Enum>(Key, ref KeyTemp, hookCtx, true, context))
			{
				KeyTemp = Key;
			}
			target.Key = KeyTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ToggleIntrinsicUIEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref InstantActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ToggleIntrinsicUIEvent cast = (ToggleIntrinsicUIEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ToggleIntrinsicUIEvent cast = (ToggleIntrinsicUIEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ToggleIntrinsicUIEvent Instantiate()
	{
		return new ToggleIntrinsicUIEvent();
	}
}
