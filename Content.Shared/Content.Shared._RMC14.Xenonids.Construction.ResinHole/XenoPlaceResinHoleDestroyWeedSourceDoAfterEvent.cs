using System;
using Content.Shared.DoAfter;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Xenonids.Construction.ResinHole;

[Serializable]
[NetSerializable]
public sealed class XenoPlaceResinHoleDestroyWeedSourceDoAfterEvent : SimpleDoAfterEvent, ISerializationGenerated<XenoPlaceResinHoleDestroyWeedSourceDoAfterEvent>, ISerializationGenerated
{
	public EntProtoId ResinHolePrototype;

	public int PlasmaCost;

	public XenoPlaceResinHoleDestroyWeedSourceDoAfterEvent(EntProtoId resinHolePrototype, int plasmaCost)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		ResinHolePrototype = resinHolePrototype;
		PlasmaCost = plasmaCost;
	}

	public XenoPlaceResinHoleDestroyWeedSourceDoAfterEvent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoPlaceResinHoleDestroyWeedSourceDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoPlaceResinHoleDestroyWeedSourceDoAfterEvent)definitionCast;
		serialization.TryCustomCopy<XenoPlaceResinHoleDestroyWeedSourceDoAfterEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoPlaceResinHoleDestroyWeedSourceDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoPlaceResinHoleDestroyWeedSourceDoAfterEvent cast = (XenoPlaceResinHoleDestroyWeedSourceDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoPlaceResinHoleDestroyWeedSourceDoAfterEvent cast = (XenoPlaceResinHoleDestroyWeedSourceDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoPlaceResinHoleDestroyWeedSourceDoAfterEvent Instantiate()
	{
		return new XenoPlaceResinHoleDestroyWeedSourceDoAfterEvent();
	}
}
