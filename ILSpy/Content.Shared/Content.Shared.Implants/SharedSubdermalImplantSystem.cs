using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Actions;
using Content.Shared.Implants.Components;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs;
using Content.Shared.Tag;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;

namespace Content.Shared.Implants;

public abstract class SharedSubdermalImplantSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actionsSystem;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private TagSystem _tag;

	[Dependency]
	private SharedTransformSystem _transformSystem;

	public const string BaseStorageId = "storagebase";

	private static readonly ProtoId<TagPrototype> MicroBombTag = ProtoId<TagPrototype>.op_Implicit("MicroBomb");

	private static readonly ProtoId<TagPrototype> MacroBombTag = ProtoId<TagPrototype>.op_Implicit("MacroBomb");

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<SubdermalImplantComponent, EntGotInsertedIntoContainerMessage>((ComponentEventHandler<SubdermalImplantComponent, EntGotInsertedIntoContainerMessage>)OnInsert, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SubdermalImplantComponent, ContainerGettingRemovedAttemptEvent>((ComponentEventHandler<SubdermalImplantComponent, ContainerGettingRemovedAttemptEvent>)OnRemoveAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SubdermalImplantComponent, EntGotRemovedFromContainerMessage>((ComponentEventHandler<SubdermalImplantComponent, EntGotRemovedFromContainerMessage>)OnRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ImplantedComponent, MobStateChangedEvent>((ComponentEventHandler<ImplantedComponent, MobStateChangedEvent>)RelayToImplantEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ImplantedComponent, AfterInteractUsingEvent>((ComponentEventHandler<ImplantedComponent, AfterInteractUsingEvent>)RelayToImplantEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ImplantedComponent, SuicideEvent>((ComponentEventHandler<ImplantedComponent, SuicideEvent>)RelayToImplantEvent, (Type[])null, (Type[])null);
	}

	private void OnInsert(EntityUid uid, SubdermalImplantComponent component, EntGotInsertedIntoContainerMessage args)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		if (!component.ImplantedEntity.HasValue)
		{
			return;
		}
		EntProtoId? implantAction = component.ImplantAction;
		if (!string.IsNullOrWhiteSpace(implantAction.HasValue ? EntProtoId.op_Implicit(implantAction.GetValueOrDefault()) : null))
		{
			SharedActionsSystem actionsSystem = _actionsSystem;
			EntityUid value = component.ImplantedEntity.Value;
			ref EntityUid? action = ref component.Action;
			implantAction = component.ImplantAction;
			actionsSystem.AddAction(value, ref action, implantAction.HasValue ? EntProtoId.op_Implicit(implantAction.GetValueOrDefault()) : null, uid);
		}
		BaseContainer implantContainer = default(BaseContainer);
		if (_container.TryGetContainer(component.ImplantedEntity.Value, "implant", ref implantContainer, (ContainerManagerComponent)null) && _tag.HasTag(uid, MacroBombTag))
		{
			foreach (EntityUid implant in implantContainer.ContainedEntities)
			{
				if (_tag.HasTag(implant, MicroBombTag))
				{
					_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(implant), implantContainer, true, false, (EntityCoordinates?)null, (Angle?)null);
					((EntitySystem)this).PredictedQueueDel(implant);
				}
			}
		}
		ImplantImplantedEvent ev = new ImplantImplantedEvent(uid, component.ImplantedEntity.Value);
		((EntitySystem)this).RaiseLocalEvent<ImplantImplantedEvent>(uid, ref ev, false);
	}

	private void OnRemoveAttempt(EntityUid uid, SubdermalImplantComponent component, ContainerGettingRemovedAttemptEvent args)
	{
		if (component.Permanent && component.ImplantedEntity.HasValue)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnRemove(EntityUid uid, SubdermalImplantComponent component, EntGotRemovedFromContainerMessage args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		if (!component.ImplantedEntity.HasValue || ((EntitySystem)this).Terminating(component.ImplantedEntity.Value, (MetaDataComponent)null))
		{
			return;
		}
		if (component.ImplantAction.HasValue)
		{
			_actionsSystem.RemoveProvidedActions(component.ImplantedEntity.Value, uid);
		}
		BaseContainer storageImplant = default(BaseContainer);
		if (_container.TryGetContainer(uid, "storagebase", ref storageImplant, (ContainerManagerComponent)null))
		{
			EntityUid[] array = storageImplant.ContainedEntities.ToArray();
			foreach (EntityUid entity in array)
			{
				_transformSystem.DropNextTo(Entity<TransformComponent>.op_Implicit(entity), Entity<TransformComponent>.op_Implicit(uid));
			}
		}
	}

	public void AddImplants(EntityUid uid, IEnumerable<string> implants)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		foreach (string id in implants)
		{
			AddImplant(uid, id);
		}
	}

	public EntityUid? AddImplant(EntityUid uid, string implantId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		EntityCoordinates coords = ((EntitySystem)this).Transform(uid).Coordinates;
		EntityUid ent = ((EntitySystem)this).Spawn(implantId, coords);
		SubdermalImplantComponent implant = default(SubdermalImplantComponent);
		if (((EntitySystem)this).TryComp<SubdermalImplantComponent>(ent, ref implant))
		{
			ForceImplant(uid, ent, implant);
			return ent;
		}
		((EntitySystem)this).Log.Warning($"Found invalid starting implant '{implantId}' on {uid} {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)):implanted}");
		((EntitySystem)this).Del((EntityUid?)ent);
		return null;
	}

	public void ForceImplant(EntityUid target, EntityUid implant, SubdermalImplantComponent component)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Container implantContainer = ((EntitySystem)this).EnsureComp<ImplantedComponent>(target).ImplantContainer;
		component.ImplantedEntity = target;
		_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(implant), (BaseContainer)(object)implantContainer, (TransformComponent)null, false);
	}

	public void ForceRemove(EntityUid target, EntityUid implant)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		ImplantedComponent implanted = default(ImplantedComponent);
		if (((EntitySystem)this).TryComp<ImplantedComponent>(target, ref implanted))
		{
			Container implantContainer = implanted.ImplantContainer;
			_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(implant), (BaseContainer)(object)implantContainer, true, false, (EntityCoordinates?)null, (Angle?)null);
			((EntitySystem)this).QueueDel((EntityUid?)implant);
		}
	}

	public void WipeImplants(EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ImplantedComponent implanted = default(ImplantedComponent);
		if (((EntitySystem)this).TryComp<ImplantedComponent>(target, ref implanted))
		{
			Container implantContainer = implanted.ImplantContainer;
			_container.CleanContainer((BaseContainer)(object)implantContainer);
		}
	}

	private void RelayToImplantEvent<T>(EntityUid uid, ImplantedComponent component, T args) where T : notnull
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer implantContainer = default(BaseContainer);
		if (!_container.TryGetContainer(uid, "implant", ref implantContainer, (ContainerManagerComponent)null))
		{
			return;
		}
		ImplantRelayEvent<T> relayEv = new ImplantRelayEvent<T>(args);
		foreach (EntityUid implant in implantContainer.ContainedEntities)
		{
			HandledEntityEventArgs e = (HandledEntityEventArgs)((((object)args) is HandledEntityEventArgs) ? ((object)args) : null);
			if (e != null && e.Handled)
			{
				break;
			}
			((EntitySystem)this).RaiseLocalEvent<ImplantRelayEvent<T>>(implant, relayEv, false);
		}
	}
}
