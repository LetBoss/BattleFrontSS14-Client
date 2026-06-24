using System;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Xenonids.Construction.Events;

[Serializable]
[NetSerializable]
public sealed class XenoConstructionAddPlasmaDoAfterEvent : DoAfterEvent, ISerializationGenerated<XenoConstructionAddPlasmaDoAfterEvent>, ISerializationGenerated
{
	public override DoAfterEvent Clone()
	{
		return this;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoConstructionAddPlasmaDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoConstructionAddPlasmaDoAfterEvent)definitionCast;
		serialization.TryCustomCopy<XenoConstructionAddPlasmaDoAfterEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoConstructionAddPlasmaDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref DoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoConstructionAddPlasmaDoAfterEvent cast = (XenoConstructionAddPlasmaDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoConstructionAddPlasmaDoAfterEvent cast = (XenoConstructionAddPlasmaDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoConstructionAddPlasmaDoAfterEvent Instantiate()
	{
		return new XenoConstructionAddPlasmaDoAfterEvent();
	}
}
