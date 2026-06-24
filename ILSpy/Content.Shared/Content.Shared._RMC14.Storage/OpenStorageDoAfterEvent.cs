using System;
using Content.Shared.DoAfter;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Storage;

[Serializable]
[NetSerializable]
public sealed class OpenStorageDoAfterEvent : DoAfterEvent, ISerializationGenerated<OpenStorageDoAfterEvent>, ISerializationGenerated
{
	public readonly NetEntity Uid;

	public readonly NetEntity Entity;

	public readonly bool Silent;

	public OpenStorageDoAfterEvent(NetEntity uid, NetEntity entity, bool silent)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Uid = uid;
		Entity = entity;
		Silent = silent;
	}

	public override DoAfterEvent Clone()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return new OpenStorageDoAfterEvent(Uid, Entity, Silent);
	}

	public OpenStorageDoAfterEvent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref OpenStorageDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (OpenStorageDoAfterEvent)definitionCast;
		serialization.TryCustomCopy<OpenStorageDoAfterEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref OpenStorageDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref DoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OpenStorageDoAfterEvent cast = (OpenStorageDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OpenStorageDoAfterEvent cast = (OpenStorageDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override OpenStorageDoAfterEvent Instantiate()
	{
		return new OpenStorageDoAfterEvent();
	}
}
