using System;
using System.Collections.Generic;
using System.Linq;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.Containers;

[ImplicitDataDefinitionForInheritors]
public abstract class BaseContainer : ISerializationGenerated<BaseContainer>, ISerializationGenerated
{
	protected SharedContainerSystem? System;

	[NonSerialized]
	[ViewVariables]
	public List<NetEntity> ExpectedEntities = new List<NetEntity>();

	[NonSerialized]
	[ViewVariables]
	[Access(new Type[]
	{
		typeof(SharedContainerSystem),
		typeof(ContainerManagerComponent)
	})]
	public string ID;

	[NonSerialized]
	protected internal ContainerManagerComponent Manager;

	public abstract IReadOnlyList<EntityUid> ContainedEntities { get; }

	[ViewVariables]
	private IReadOnlyList<NetEntity> NetContainedEntities => ContainedEntities.Select((EntityUid o) => IoCManager.Resolve<IEntityManager>().GetNetEntity(o)).ToList();

	public abstract int Count { get; }

	[ViewVariables(VVAccess.ReadWrite)]
	[DataField("occludes", false, 1, false, false, null)]
	public bool OccludesLight { get; set; } = true;

	[ViewVariables]
	public EntityUid Owner { get; internal set; }

	[ViewVariables(VVAccess.ReadWrite)]
	[DataField("showEnts", false, 1, false, false, null)]
	public bool ShowContents { get; set; }

	[Access(new Type[]
	{
		typeof(SharedContainerSystem),
		typeof(ContainerManagerComponent)
	})]
	internal void Init(SharedContainerSystem system, string id, Entity<ContainerManagerComponent> owner)
	{
		ID = id;
		Owner = owner;
		Manager = owner;
		System = system;
	}

	public abstract bool Contains(EntityUid contained);

	protected internal virtual bool CanInsert(EntityUid toInsert, bool assumeEmpty, IEntityManager entMan)
	{
		return true;
	}

	[Access(new Type[] { typeof(SharedContainerSystem) })]
	protected internal abstract void InternalInsert(EntityUid toInsert, IEntityManager entMan);

	[Access(new Type[] { typeof(SharedContainerSystem) })]
	protected internal abstract void InternalRemove(EntityUid toRemove, IEntityManager entMan);

	[Access(new Type[] { typeof(SharedContainerSystem) })]
	protected internal abstract void InternalShutdown(IEntityManager entMan, SharedContainerSystem system, bool isClient);

	public BaseContainer()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref BaseContainer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			bool target2 = false;
			if (!serialization.TryCustomCopy(OccludesLight, ref target2, hookCtx, hasHooks: false, context))
			{
				target2 = OccludesLight;
			}
			target.OccludesLight = target2;
			bool target3 = false;
			if (!serialization.TryCustomCopy(ShowContents, ref target3, hookCtx, hasHooks: false, context))
			{
				target3 = ShowContents;
			}
			target.ShowContents = target3;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref BaseContainer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BaseContainer target2 = (BaseContainer)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public virtual BaseContainer Instantiate()
	{
		throw new NotImplementedException();
	}
}
