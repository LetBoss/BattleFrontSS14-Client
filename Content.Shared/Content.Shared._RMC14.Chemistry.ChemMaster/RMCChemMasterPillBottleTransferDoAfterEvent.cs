using System;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Chemistry.ChemMaster;

[Serializable]
[NetSerializable]
public sealed class RMCChemMasterPillBottleTransferDoAfterEvent : SimpleDoAfterEvent, ISerializationGenerated<RMCChemMasterPillBottleTransferDoAfterEvent>, ISerializationGenerated
{
	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCChemMasterPillBottleTransferDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RMCChemMasterPillBottleTransferDoAfterEvent)definitionCast;
		serialization.TryCustomCopy<RMCChemMasterPillBottleTransferDoAfterEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCChemMasterPillBottleTransferDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCChemMasterPillBottleTransferDoAfterEvent cast = (RMCChemMasterPillBottleTransferDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCChemMasterPillBottleTransferDoAfterEvent cast = (RMCChemMasterPillBottleTransferDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RMCChemMasterPillBottleTransferDoAfterEvent Instantiate()
	{
		return new RMCChemMasterPillBottleTransferDoAfterEvent();
	}
}
