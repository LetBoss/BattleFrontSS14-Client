using System;
using Content.Shared.Actions;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Xenonids.Construction.Tunnel;

public sealed class XenoDigTunnelActionEvent : InstantActionEvent, ISerializationGenerated<XenoDigTunnelActionEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntProtoId Prototype = EntProtoId.op_Implicit("XenoTunnel");

	[DataField(null, false, 1, false, false, null)]
	public float DestroyWeedSourceDelay = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float CreateTunnelDelay = 4f;

	[DataField(null, false, 1, false, false, null)]
	public int PlasmaCost = 200;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoDigTunnelActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		InstantActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoDigTunnelActionEvent)definitionCast;
		if (!serialization.TryCustomCopy<XenoDigTunnelActionEvent>(this, ref target, hookCtx, false, context))
		{
			EntProtoId PrototypeTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(Prototype, ref PrototypeTemp, hookCtx, false, context))
			{
				PrototypeTemp = serialization.CreateCopy<EntProtoId>(Prototype, hookCtx, context, false);
			}
			target.Prototype = PrototypeTemp;
			float DestroyWeedSourceDelayTemp = 0f;
			if (!serialization.TryCustomCopy<float>(DestroyWeedSourceDelay, ref DestroyWeedSourceDelayTemp, hookCtx, false, context))
			{
				DestroyWeedSourceDelayTemp = DestroyWeedSourceDelay;
			}
			target.DestroyWeedSourceDelay = DestroyWeedSourceDelayTemp;
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
	public void Copy(ref XenoDigTunnelActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref InstantActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoDigTunnelActionEvent cast = (XenoDigTunnelActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoDigTunnelActionEvent cast = (XenoDigTunnelActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoDigTunnelActionEvent Instantiate()
	{
		return new XenoDigTunnelActionEvent();
	}
}
