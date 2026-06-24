using System;
using Content.Shared.DoAfter;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Weapons.Ranged;

[Serializable]
[NetSerializable]
public sealed class AirShotDoAfterEvent : SimpleDoAfterEvent, ISerializationGenerated<AirShotDoAfterEvent>, ISerializationGenerated
{
	public readonly NetCoordinates Coordinates;

	public AirShotDoAfterEvent(NetCoordinates coordinates)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Coordinates = coordinates;
	}

	public AirShotDoAfterEvent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AirShotDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (AirShotDoAfterEvent)definitionCast;
		serialization.TryCustomCopy<AirShotDoAfterEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AirShotDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AirShotDoAfterEvent cast = (AirShotDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AirShotDoAfterEvent cast = (AirShotDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override AirShotDoAfterEvent Instantiate()
	{
		return new AirShotDoAfterEvent();
	}
}
