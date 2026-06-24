using System;
using Content.Shared.DoAfter;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Xenonids.Construction.Tunnel;

[Serializable]
[NetSerializable]
public sealed class XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent : SimpleDoAfterEvent, ISerializationGenerated<XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntProtoId Prototype = EntProtoId.op_Implicit("XenoTunnel");

	[DataField(null, false, 1, false, false, null)]
	public float CreateTunnelDelay = 4f;

	[DataField(null, false, 1, false, false, null)]
	public int PlasmaCost = 200;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent)definitionCast;
		if (!serialization.TryCustomCopy<XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent>(this, ref target, hookCtx, false, context))
		{
			EntProtoId PrototypeTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(Prototype, ref PrototypeTemp, hookCtx, false, context))
			{
				PrototypeTemp = serialization.CreateCopy<EntProtoId>(Prototype, hookCtx, context, false);
			}
			target.Prototype = PrototypeTemp;
			float CreateTunnelDelayTemp = 0f;
			if (!serialization.TryCustomCopy<float>(CreateTunnelDelay, ref CreateTunnelDelayTemp, hookCtx, false, context))
			{
				CreateTunnelDelayTemp = CreateTunnelDelay;
			}
			target.CreateTunnelDelay = CreateTunnelDelayTemp;
			int PlasmaCostTemp = 0;
			if (!serialization.TryCustomCopy<int>(PlasmaCost, ref PlasmaCostTemp, hookCtx, false, context))
			{
				PlasmaCostTemp = PlasmaCost;
			}
			target.PlasmaCost = PlasmaCostTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent cast = (XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent cast = (XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent Instantiate()
	{
		return new XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent();
	}
}
