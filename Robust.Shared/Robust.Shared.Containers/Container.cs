using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Robust.Shared.Containers;

[SerializedType("Container")]
public sealed class Container : BaseContainer, ISerializationGenerated<Container>, ISerializationGenerated
{
	[NonSerialized]
	[DataField("ents", false, 1, false, false, null)]
	private List<EntityUid> _containerList = new List<EntityUid>();

	public override int Count => _containerList.Count;

	public override IReadOnlyList<EntityUid> ContainedEntities => _containerList;

	protected internal override void InternalInsert(EntityUid toInsert, IEntityManager entMan)
	{
		_containerList.Add(toInsert);
	}

	protected internal override void InternalRemove(EntityUid toRemove, IEntityManager entMan)
	{
		_containerList.Remove(toRemove);
	}

	public override bool Contains(EntityUid contained)
	{
		if (!_containerList.Contains(contained))
		{
			return false;
		}
		return true;
	}

	protected internal override void InternalShutdown(IEntityManager entMan, SharedContainerSystem system, bool isClient)
	{
		EntityUid[] array = _containerList.ToArray();
		foreach (EntityUid entityUid in array)
		{
			if (!isClient)
			{
				entMan.DeleteEntity(entityUid);
			}
			else if (entMan.EntityExists(entityUid))
			{
				system.Remove(entityUid, this, reparent: false, force: true);
			}
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref Container target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BaseContainer target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (Container)target2;
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			List<EntityUid> target3 = null;
			if (_containerList == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy(_containerList, ref target3, hookCtx, hasHooks: true, context))
			{
				target3 = serialization.CreateCopy(_containerList, hookCtx, context);
			}
			target._containerList = target3;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref Container target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref BaseContainer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Container target2 = (Container)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Container target2 = (Container)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override Container Instantiate()
	{
		return new Container();
	}
}
