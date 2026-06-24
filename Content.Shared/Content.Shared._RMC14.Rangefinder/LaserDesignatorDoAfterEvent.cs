using System;
using Content.Shared.DoAfter;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Rangefinder;

[Serializable]
[NetSerializable]
public sealed class LaserDesignatorDoAfterEvent : SimpleDoAfterEvent, ISerializationGenerated<LaserDesignatorDoAfterEvent>, ISerializationGenerated
{
	public readonly NetCoordinates Coordinates;

	public LaserDesignatorDoAfterEvent(NetCoordinates coordinates)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Coordinates = coordinates;
	}

	public LaserDesignatorDoAfterEvent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref LaserDesignatorDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (LaserDesignatorDoAfterEvent)definitionCast;
		serialization.TryCustomCopy<LaserDesignatorDoAfterEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref LaserDesignatorDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LaserDesignatorDoAfterEvent cast = (LaserDesignatorDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LaserDesignatorDoAfterEvent cast = (LaserDesignatorDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override LaserDesignatorDoAfterEvent Instantiate()
	{
		return new LaserDesignatorDoAfterEvent();
	}
}
