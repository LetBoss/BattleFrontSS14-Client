using System;
using Content.Shared.DoAfter;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Xenonids.Destroy;

[Serializable]
[NetSerializable]
public sealed class XenoDestroyLeapDoafter : SimpleDoAfterEvent, ISerializationGenerated<XenoDestroyLeapDoafter>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public NetCoordinates TargetCoords;

	public XenoDestroyLeapDoafter(NetCoordinates coordinates)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		TargetCoords = coordinates;
	}

	public XenoDestroyLeapDoafter()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoDestroyLeapDoafter target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
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
		target = (XenoDestroyLeapDoafter)definitionCast;
		if (!serialization.TryCustomCopy<XenoDestroyLeapDoafter>(this, ref target, hookCtx, false, context))
		{
			NetCoordinates TargetCoordsTemp = default(NetCoordinates);
			if (!serialization.TryCustomCopy<NetCoordinates>(TargetCoords, ref TargetCoordsTemp, hookCtx, false, context))
			{
				TargetCoordsTemp = serialization.CreateCopy<NetCoordinates>(TargetCoords, hookCtx, context, false);
			}
			target.TargetCoords = TargetCoordsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoDestroyLeapDoafter target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoDestroyLeapDoafter cast = (XenoDestroyLeapDoafter)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoDestroyLeapDoafter cast = (XenoDestroyLeapDoafter)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoDestroyLeapDoafter Instantiate()
	{
		return new XenoDestroyLeapDoafter();
	}
}
