using System;
using Content.Shared.DoAfter;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Mortar;

[Serializable]
[NetSerializable]
public sealed class MortarLaserTargetUpdateDoAfterEvent : DoAfterEvent, ISerializationGenerated<MortarLaserTargetUpdateDoAfterEvent>, ISerializationGenerated
{
	public NetCoordinates TargetCoordinates;

	public MortarLaserTargetUpdateDoAfterEvent(NetCoordinates targetCoordinates)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		TargetCoordinates = targetCoordinates;
	}

	public override DoAfterEvent Clone()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return new MortarLaserTargetUpdateDoAfterEvent(TargetCoordinates);
	}

	public MortarLaserTargetUpdateDoAfterEvent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MortarLaserTargetUpdateDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (MortarLaserTargetUpdateDoAfterEvent)definitionCast;
		serialization.TryCustomCopy<MortarLaserTargetUpdateDoAfterEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MortarLaserTargetUpdateDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref DoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MortarLaserTargetUpdateDoAfterEvent cast = (MortarLaserTargetUpdateDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MortarLaserTargetUpdateDoAfterEvent cast = (MortarLaserTargetUpdateDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MortarLaserTargetUpdateDoAfterEvent Instantiate()
	{
		return new MortarLaserTargetUpdateDoAfterEvent();
	}
}
