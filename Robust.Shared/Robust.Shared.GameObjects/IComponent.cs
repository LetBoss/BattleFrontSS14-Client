using System;
using Robust.Shared.Analyzers;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;

namespace Robust.Shared.GameObjects;

[ImplicitDataDefinitionForInheritors]
[NotContentImplementable]
public interface IComponent : ISerializationGenerated<IComponent>, ISerializationGenerated
{
	ComponentLifeStage LifeStage { get; internal set; }

	internal bool Networked { get; set; }

	bool NetSyncEnabled { get; set; }

	bool SendOnlyToOwner { get; }

	bool SessionSpecific { get; }

	[Obsolete("Update your API to allow accessing Owner through other means")]
	EntityUid Owner { get; set; }

	bool Initialized { get; }

	bool Running { get; }

	bool Deleted { get; }

	GameTick CreationTick { get; internal set; }

	GameTick LastModifiedTick { get; internal set; }

	[Obsolete]
	void Dirty(IEntityManager? entManager = null);

	internal void ClearTicks();

	internal void ClearCreationTick();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	new void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	new void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	new void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IComponent target2 = (IComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	new IComponent Instantiate()
	{
		throw new NotImplementedException();
	}
}
