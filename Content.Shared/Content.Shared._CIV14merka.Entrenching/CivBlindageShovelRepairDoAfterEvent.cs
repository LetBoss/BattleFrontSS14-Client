using System;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._CIV14merka.Entrenching;

[Serializable]
[NetSerializable]
public sealed class CivBlindageShovelRepairDoAfterEvent : SimpleDoAfterEvent, ISerializationGenerated<CivBlindageShovelRepairDoAfterEvent>, ISerializationGenerated
{
	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CivBlindageShovelRepairDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (CivBlindageShovelRepairDoAfterEvent)definitionCast;
		serialization.TryCustomCopy<CivBlindageShovelRepairDoAfterEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CivBlindageShovelRepairDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CivBlindageShovelRepairDoAfterEvent cast = (CivBlindageShovelRepairDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CivBlindageShovelRepairDoAfterEvent cast = (CivBlindageShovelRepairDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CivBlindageShovelRepairDoAfterEvent Instantiate()
	{
		return new CivBlindageShovelRepairDoAfterEvent();
	}
}
