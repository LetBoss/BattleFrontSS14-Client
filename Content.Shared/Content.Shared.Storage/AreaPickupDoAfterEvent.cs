using System;
using System.Collections.Generic;
using Content.Shared.DoAfter;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Storage;

[Serializable]
[NetSerializable]
public sealed class AreaPickupDoAfterEvent : DoAfterEvent, ISerializationGenerated<AreaPickupDoAfterEvent>, ISerializationGenerated
{
	[DataField("entities", false, 1, true, false, null)]
	public IReadOnlyList<NetEntity> Entities;

	private AreaPickupDoAfterEvent()
	{
	}

	public AreaPickupDoAfterEvent(List<NetEntity> entities)
	{
		Entities = entities;
	}

	public override DoAfterEvent Clone()
	{
		return this;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AreaPickupDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		DoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (AreaPickupDoAfterEvent)definitionCast;
		if (!serialization.TryCustomCopy<AreaPickupDoAfterEvent>(this, ref target, hookCtx, false, context))
		{
			IReadOnlyList<NetEntity> EntitiesTemp = null;
			if (Entities == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<IReadOnlyList<NetEntity>>(Entities, ref EntitiesTemp, hookCtx, true, context))
			{
				EntitiesTemp = serialization.CreateCopy<IReadOnlyList<NetEntity>>(Entities, hookCtx, context, false);
			}
			target.Entities = EntitiesTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AreaPickupDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref DoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AreaPickupDoAfterEvent cast = (AreaPickupDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AreaPickupDoAfterEvent cast = (AreaPickupDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override AreaPickupDoAfterEvent Instantiate()
	{
		return new AreaPickupDoAfterEvent();
	}
}
