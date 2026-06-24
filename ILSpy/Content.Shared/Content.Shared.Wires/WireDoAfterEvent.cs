using System;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Wires;

[Serializable]
[NetSerializable]
public sealed class WireDoAfterEvent : DoAfterEvent, ISerializationGenerated<WireDoAfterEvent>, ISerializationGenerated
{
	[DataField("action", false, 1, true, false, null)]
	public WiresAction Action;

	[DataField("id", false, 1, true, false, null)]
	public int Id;

	private WireDoAfterEvent()
	{
	}

	public WireDoAfterEvent(WiresAction action, int id)
	{
		Action = action;
		Id = id;
	}

	public override DoAfterEvent Clone()
	{
		return this;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref WireDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (WireDoAfterEvent)definitionCast;
		if (!serialization.TryCustomCopy<WireDoAfterEvent>(this, ref target, hookCtx, false, context))
		{
			WiresAction ActionTemp = WiresAction.Mend;
			if (!serialization.TryCustomCopy<WiresAction>(Action, ref ActionTemp, hookCtx, false, context))
			{
				ActionTemp = Action;
			}
			target.Action = ActionTemp;
			int IdTemp = 0;
			if (!serialization.TryCustomCopy<int>(Id, ref IdTemp, hookCtx, false, context))
			{
				IdTemp = Id;
			}
			target.Id = IdTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref WireDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref DoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WireDoAfterEvent cast = (WireDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WireDoAfterEvent cast = (WireDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override WireDoAfterEvent Instantiate()
	{
		return new WireDoAfterEvent();
	}
}
