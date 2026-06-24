using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.PowerLoader.Events;

[Serializable]
[NetSerializable]
public sealed class DropshipAttachDoAfterEvent : DropshipDoAfterEvent, ISerializationGenerated<DropshipAttachDoAfterEvent>, ISerializationGenerated
{
	public DropshipAttachDoAfterEvent(NetEntity container, NetEntity contained, string slot)
		: base(container, contained, slot)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0002: Unknown result type (might be due to invalid IL or missing references)


	public DropshipAttachDoAfterEvent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DropshipAttachDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DropshipDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (DropshipAttachDoAfterEvent)definitionCast;
		serialization.TryCustomCopy<DropshipAttachDoAfterEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DropshipAttachDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref DropshipDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DropshipAttachDoAfterEvent cast = (DropshipAttachDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DropshipAttachDoAfterEvent cast = (DropshipAttachDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override DropshipAttachDoAfterEvent Instantiate()
	{
		return new DropshipAttachDoAfterEvent();
	}
}
