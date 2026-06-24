using System;
using Content.Shared.DoAfter;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Xenonids.Construction.Tunnel;

[Serializable]
[NetSerializable]
public sealed class XenoDigTunnelDoAfter : SimpleDoAfterEvent, ISerializationGenerated<XenoDigTunnelDoAfter>, ISerializationGenerated
{
	public int PlasmaCost = 200;

	public string Prototype;

	public XenoDigTunnelDoAfter(EntProtoId prototype, int plasmaCost)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		PlasmaCost = plasmaCost;
		Prototype = EntProtoId.op_Implicit(prototype);
	}

	public XenoDigTunnelDoAfter()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoDigTunnelDoAfter target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoDigTunnelDoAfter)definitionCast;
		serialization.TryCustomCopy<XenoDigTunnelDoAfter>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoDigTunnelDoAfter target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoDigTunnelDoAfter cast = (XenoDigTunnelDoAfter)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoDigTunnelDoAfter cast = (XenoDigTunnelDoAfter)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoDigTunnelDoAfter Instantiate()
	{
		return new XenoDigTunnelDoAfter();
	}
}
