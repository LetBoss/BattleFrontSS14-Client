using System;
using Content.Shared.DoAfter;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Mortar;

[Serializable]
[NetSerializable]
public sealed class LinkMortarLaserDesignatorDoAfterEvent : SimpleDoAfterEvent, ISerializationGenerated<LinkMortarLaserDesignatorDoAfterEvent>, ISerializationGenerated
{
	public NetEntity LaserDesignator;

	public LinkMortarLaserDesignatorDoAfterEvent(NetEntity laserDesignator)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		LaserDesignator = laserDesignator;
	}

	public LinkMortarLaserDesignatorDoAfterEvent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref LinkMortarLaserDesignatorDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (LinkMortarLaserDesignatorDoAfterEvent)definitionCast;
		serialization.TryCustomCopy<LinkMortarLaserDesignatorDoAfterEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref LinkMortarLaserDesignatorDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LinkMortarLaserDesignatorDoAfterEvent cast = (LinkMortarLaserDesignatorDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LinkMortarLaserDesignatorDoAfterEvent cast = (LinkMortarLaserDesignatorDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override LinkMortarLaserDesignatorDoAfterEvent Instantiate()
	{
		return new LinkMortarLaserDesignatorDoAfterEvent();
	}
}
