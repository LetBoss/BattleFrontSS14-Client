using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Robust.Shared.Containers;

[SerializedType("ContainerSlot")]
public sealed class ContainerSlot : BaseContainer, ISerializationGenerated<ContainerSlot>, ISerializationGenerated
{
	[NonSerialized]
	private EntityUid? _containedEntity;

	[NonSerialized]
	private EntityUid[]? _containedEntityArray;

	public override int Count => ContainedEntity.HasValue ? 1 : 0;

	public override IReadOnlyList<EntityUid> ContainedEntities
	{
		get
		{
			if (!_containedEntity.HasValue)
			{
				return Array.Empty<EntityUid>();
			}
			if (_containedEntityArray == null)
			{
				_containedEntityArray = new EntityUid[1] { _containedEntity.Value };
			}
			return _containedEntityArray;
		}
	}

	[DataField("ent", false, 1, false, false, null)]
	public EntityUid? ContainedEntity
	{
		get
		{
			return _containedEntity;
		}
		private set
		{
			_containedEntity = value;
			if (value.HasValue)
			{
				if (_containedEntityArray == null)
				{
					_containedEntityArray = new EntityUid[1];
				}
				_containedEntityArray[0] = value.Value;
			}
		}
	}

	public override bool Contains(EntityUid contained)
	{
		EntityUid? containedEntity = ContainedEntity;
		if (contained != containedEntity)
		{
			return false;
		}
		return true;
	}

	protected internal override bool CanInsert(EntityUid toInsert, bool assumeEmpty, IEntityManager entMan)
	{
		return !ContainedEntity.HasValue || assumeEmpty;
	}

	protected internal override void InternalInsert(EntityUid toInsert, IEntityManager entMan)
	{
		ContainedEntity = toInsert;
	}

	protected internal override void InternalRemove(EntityUid toRemove, IEntityManager entMan)
	{
		ContainedEntity = null;
	}

	protected internal override void InternalShutdown(IEntityManager entMan, SharedContainerSystem system, bool isClient)
	{
		EntityUid? containedEntity = ContainedEntity;
		if (containedEntity.HasValue)
		{
			EntityUid valueOrDefault = containedEntity.GetValueOrDefault();
			if (!isClient)
			{
				entMan.DeleteEntity(valueOrDefault);
			}
			else if (entMan.EntityExists(valueOrDefault))
			{
				system.Remove(valueOrDefault, this, reparent: false, force: true);
			}
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ContainerSlot target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BaseContainer target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (ContainerSlot)target2;
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			EntityUid? target3 = null;
			if (!serialization.TryCustomCopy(ContainedEntity, ref target3, hookCtx, hasHooks: false, context))
			{
				target3 = serialization.CreateCopy(ContainedEntity, hookCtx, context);
			}
			target.ContainedEntity = target3;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ContainerSlot target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref BaseContainer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ContainerSlot target2 = (ContainerSlot)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ContainerSlot target2 = (ContainerSlot)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ContainerSlot Instantiate()
	{
		return new ContainerSlot();
	}
}
