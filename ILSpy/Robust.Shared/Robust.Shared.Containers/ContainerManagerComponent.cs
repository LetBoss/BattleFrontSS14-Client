using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Robust.Shared.Containers;

[NetworkedComponent]
[RegisterComponent]
[ComponentProtoName("ContainerContainer")]
public sealed class ContainerManagerComponent : Component, ISerializationHooks, ISerializationGenerated<ContainerManagerComponent>, ISerializationGenerated
{
	[Serializable]
	[NetSerializable]
	internal sealed class ContainerManagerComponentState : ComponentState
	{
		[Serializable]
		[NetSerializable]
		public readonly struct ContainerData(string containerType, bool showContents, bool occludesLight, NetEntity[] containedEntities)
		{
			public readonly string ContainerType = containerType;

			public readonly bool ShowContents = showContents;

			public readonly bool OccludesLight = occludesLight;

			public readonly NetEntity[] ContainedEntities = containedEntities;

			public void Deconstruct(out string type, out bool showEnts, out bool occludesLight, out NetEntity[] ents)
			{
				type = ContainerType;
				showEnts = ShowContents;
				occludesLight = OccludesLight;
				ents = ContainedEntities;
			}
		}

		public Dictionary<string, ContainerData> Containers;

		public ContainerManagerComponentState(Dictionary<string, ContainerData> containers)
		{
			Containers = containers;
		}
	}

	public readonly struct AllContainersEnumerable(ContainerManagerComponent? manager) : IEnumerable<BaseContainer>, IEnumerable
	{
		private readonly ContainerManagerComponent? _manager = manager;

		public AllContainersEnumerator GetEnumerator()
		{
			return new AllContainersEnumerator(_manager);
		}

		IEnumerator<BaseContainer> IEnumerable<BaseContainer>.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	public struct AllContainersEnumerator(ContainerManagerComponent? manager) : IEnumerator<BaseContainer>, IEnumerator, IDisposable
	{
		private Dictionary<string, BaseContainer>.ValueCollection.Enumerator _enumerator = manager?.Containers.Values.GetEnumerator() ?? default(Dictionary<string, BaseContainer>.ValueCollection.Enumerator);

		public BaseContainer Current
		{
			get; [param: AllowNull]
			private set;
		} = null;

		object IEnumerator.Current => Current;

		public bool MoveNext()
		{
			if (_enumerator.MoveNext())
			{
				Current = _enumerator.Current;
				return true;
			}
			return false;
		}

		void IEnumerator.Reset()
		{
			((IEnumerator)_enumerator).Reset();
		}

		public void Dispose()
		{
		}
	}

	[Dependency]
	private readonly IEntityManager _entMan;

	[DataField("containers", false, 1, false, false, null)]
	public Dictionary<string, BaseContainer> Containers = new Dictionary<string, BaseContainer>();

	void ISerializationHooks.AfterDeserialization()
	{
		foreach (var (id, baseContainer2) in Containers)
		{
			baseContainer2.Init(null, id, (Owner: base.Owner, Comp: this));
		}
	}

	[Obsolete]
	public bool TryGetContainer(string id, [NotNullWhen(true)] out BaseContainer? container)
	{
		return _entMan.System<SharedContainerSystem>().TryGetContainer(base.Owner, id, out container, this);
	}

	[Obsolete]
	public bool TryGetContainer(EntityUid entity, [NotNullWhen(true)] out BaseContainer? container)
	{
		return _entMan.System<SharedContainerSystem>().TryGetContainingContainer(base.Owner, entity, out container, this);
	}

	[Obsolete]
	public AllContainersEnumerable GetAllContainers()
	{
		return _entMan.System<SharedContainerSystem>().GetAllContainers(base.Owner, this);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ContainerManagerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (ContainerManagerComponent)target2;
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: true, context))
		{
			Dictionary<string, BaseContainer> target3 = null;
			if (Containers == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy(Containers, ref target3, hookCtx, hasHooks: true, context))
			{
				target3 = serialization.CreateCopy(Containers, hookCtx, context);
			}
			target.Containers = target3;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ContainerManagerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ContainerManagerComponent target2 = (ContainerManagerComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ContainerManagerComponent target2 = (ContainerManagerComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ContainerManagerComponent target2 = (ContainerManagerComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ContainerManagerComponent Instantiate()
	{
		return new ContainerManagerComponent();
	}
}
