using System;
using Content.Shared.DoAfter;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.PowerLoader.Events;

[Serializable]
[NetSerializable]
public abstract class DropshipDoAfterEvent : SimpleDoAfterEvent, ISerializationGenerated<DropshipDoAfterEvent>, ISerializationGenerated
{
	public NetEntity Container;

	public NetEntity Contained;

	public string Slot;

	protected DropshipDoAfterEvent(NetEntity container, NetEntity contained, string slot)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Container = container;
		Contained = contained;
		Slot = slot;
	}

	public DropshipDoAfterEvent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref DropshipDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (DropshipDoAfterEvent)definitionCast;
		serialization.TryCustomCopy<DropshipDoAfterEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref DropshipDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DropshipDoAfterEvent cast = (DropshipDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DropshipDoAfterEvent cast = (DropshipDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override DropshipDoAfterEvent Instantiate()
	{
		throw new NotImplementedException();
	}
}
