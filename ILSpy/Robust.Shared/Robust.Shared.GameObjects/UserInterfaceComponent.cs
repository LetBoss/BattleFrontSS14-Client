using System;
using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.GameObjects;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedUserInterfaceSystem) })]
public sealed class UserInterfaceComponent : Component, IComponentDelta, IComponent, ISerializationGenerated<IComponent>, ISerializationGenerated, ISerializationGenerated<IComponentDelta>, ISerializationGenerated<UserInterfaceComponent>
{
	[ViewVariables]
	[Access(new Type[] { }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.ReadWriteExecute)]
	public readonly Dictionary<Enum, BoundUserInterface> ClientOpenInterfaces = new Dictionary<Enum, BoundUserInterface>();

	[DataField(null, false, 1, false, false, null)]
	internal Dictionary<Enum, InterfaceData> Interfaces = new Dictionary<Enum, InterfaceData>();

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<Enum, HashSet<EntityUid>> Actors = new Dictionary<Enum, HashSet<EntityUid>>();

	public Dictionary<Enum, BoundUserInterfaceState> States = new Dictionary<Enum, BoundUserInterfaceState>();

	public GameTick LastFieldUpdate { get; set; }

	public GameTick[] LastModifiedFields { get; set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref UserInterfaceComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (UserInterfaceComponent)target2;
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			Dictionary<Enum, InterfaceData> target3 = null;
			if (Interfaces == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy(Interfaces, ref target3, hookCtx, hasHooks: true, context))
			{
				target3 = serialization.CreateCopy(Interfaces, hookCtx, context);
			}
			target.Interfaces = target3;
			Dictionary<Enum, HashSet<EntityUid>> target4 = null;
			if (Actors == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy(Actors, ref target4, hookCtx, hasHooks: true, context))
			{
				target4 = serialization.CreateCopy(Actors, hookCtx, context);
			}
			target.Actors = target4;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref UserInterfaceComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UserInterfaceComponent target2 = (UserInterfaceComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UserInterfaceComponent target2 = (UserInterfaceComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IComponentDelta target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UserInterfaceComponent target2 = (UserInterfaceComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IComponentDelta target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UserInterfaceComponent target2 = (UserInterfaceComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override UserInterfaceComponent Instantiate()
	{
		return new UserInterfaceComponent();
	}

	IComponentDelta IComponentDelta.Instantiate()
	{
		return Instantiate();
	}

	IComponentDelta ISerializationGenerated<IComponentDelta>.Instantiate()
	{
		return Instantiate();
	}
}
