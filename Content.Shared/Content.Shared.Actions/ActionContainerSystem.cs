using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared.Actions.Components;
using Content.Shared.Ghost;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Actions;

public sealed class ActionContainerSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private INetManager _netMan;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedMindSystem _mind;

	private EntityQuery<ActionComponent> _query;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_query = ((EntitySystem)this).GetEntityQuery<ActionComponent>();
		((EntitySystem)this).SubscribeLocalEvent<ActionsContainerComponent, ComponentInit>((ComponentEventHandler<ActionsContainerComponent, ComponentInit>)OnInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActionsContainerComponent, ComponentShutdown>((ComponentEventHandler<ActionsContainerComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActionsContainerComponent, EntRemovedFromContainerMessage>((ComponentEventHandler<ActionsContainerComponent, EntRemovedFromContainerMessage>)OnEntityRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActionsContainerComponent, EntInsertedIntoContainerMessage>((ComponentEventHandler<ActionsContainerComponent, EntInsertedIntoContainerMessage>)OnEntityInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActionsContainerComponent, ActionAddedEvent>((ComponentEventHandler<ActionsContainerComponent, ActionAddedEvent>)OnActionAdded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActionsContainerComponent, MindAddedMessage>((ComponentEventHandler<ActionsContainerComponent, MindAddedMessage>)OnMindAdded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActionsContainerComponent, MindRemovedMessage>((ComponentEventHandler<ActionsContainerComponent, MindRemovedMessage>)OnMindRemoved, (Type[])null, (Type[])null);
	}

	private void OnMindAdded(EntityUid uid, ActionsContainerComponent component, MindAddedMessage args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		ActionsContainerComponent mindActionContainerComp = default(ActionsContainerComponent);
		if (_mind.TryGetMind(uid, out EntityUid mindId, out MindComponent _) && ((EntitySystem)this).TryComp<ActionsContainerComponent>(mindId, ref mindActionContainerComp) && !((EntitySystem)this).HasComp<GhostComponent>(uid) && ((BaseContainer)mindActionContainerComp.Container).ContainedEntities.Count > 0)
		{
			_actions.GrantContainedActions(Entity<ActionsComponent>.op_Implicit(uid), Entity<ActionsContainerComponent>.op_Implicit(mindId));
		}
	}

	private void OnMindRemoved(EntityUid uid, ActionsContainerComponent component, MindRemovedMessage args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		_actions.RemoveProvidedActions(uid, Entity<MindComponent>.op_Implicit(args.Mind));
	}

	public EntityUid? AddAction(EntityUid uid, string actionPrototypeId, ActionsContainerComponent? comp = null)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? result = null;
		EnsureAction(uid, ref result, actionPrototypeId, comp);
		return result;
	}

	public bool EnsureAction(EntityUid uid, [NotNullWhen(true)] ref EntityUid? actionId, string actionPrototypeId, ActionsContainerComponent? comp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ActionComponent action;
		return EnsureAction(uid, ref actionId, out action, actionPrototypeId, comp);
	}

	public bool EnsureAction(EntityUid uid, [NotNullWhen(true)] ref EntityUid? actionId, [NotNullWhen(true)] out ActionComponent? action, string? actionPrototypeId, ActionsContainerComponent? comp = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		action = null;
		if (comp == null)
		{
			comp = ((EntitySystem)this).EnsureComp<ActionsContainerComponent>(uid);
		}
		if (((EntitySystem)this).Exists(actionId))
		{
			if (!((BaseContainer)comp.Container).Contains(actionId.Value))
			{
				((EntitySystem)this).Log.Error($"Action {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(actionId.Value))} is not contained in the expected container {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}");
				return false;
			}
			SharedActionsSystem actions = _actions;
			EntityUid? val = actionId;
			Entity<ActionComponent>? action2 = actions.GetAction(val.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(val.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
			if (action2.HasValue)
			{
				Entity<ActionComponent> ent = action2.GetValueOrDefault();
				actionId = Entity<ActionComponent>.op_Implicit(ent);
				action = ent.Comp;
				return true;
			}
			return false;
		}
		if (actionPrototypeId == null)
		{
			return false;
		}
		if (_netMan.IsClient && !((EntitySystem)this).IsClientSide(uid, (MetaDataComponent)null))
		{
			return false;
		}
		actionId = ((EntitySystem)this).Spawn(actionPrototypeId, (ComponentRegistry)null, true);
		if (!_query.TryComp(actionId, ref action))
		{
			((EntitySystem)this).Log.Error($"Tried to add invalid action {((EntitySystem)this).ToPrettyString(actionId, (MetaDataComponent)null)} to {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}!");
			((EntitySystem)this).Del(actionId);
			return false;
		}
		if (AddAction(uid, actionId.Value, action, comp))
		{
			return true;
		}
		((EntitySystem)this).Del((EntityUid?)actionId.Value);
		actionId = null;
		return false;
	}

	public void TransferAction(EntityUid actionId, EntityUid newContainer, ActionComponent? action = null, ActionsContainerComponent? container = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		Entity<ActionComponent>? action2 = _actions.GetAction(Entity<ActionComponent>.op_Implicit((actionId, action)));
		if (action2.HasValue)
		{
			Entity<ActionComponent> ent = action2.GetValueOrDefault();
			EntityUid? container2 = ent.Comp.Container;
			if (!container2.HasValue || !(container2.GetValueOrDefault() == newContainer))
			{
				_ = ent.Comp.AttachedEntity;
				AddAction(newContainer, Entity<ActionComponent>.op_Implicit(ent), ent.Comp, container);
			}
		}
	}

	public void TransferAllActions(EntityUid from, EntityUid to, ActionsContainerComponent? oldContainer = null, ActionsContainerComponent? newContainer = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ActionsContainerComponent>(from, ref oldContainer, true) && ((EntitySystem)this).Resolve<ActionsContainerComponent>(to, ref newContainer, true))
		{
			EntityUid[] array = ((BaseContainer)oldContainer.Container).ContainedEntities.ToArray();
			foreach (EntityUid action in array)
			{
				TransferAction(action, to, null, newContainer);
			}
		}
	}

	public void TransferActionWithNewAttached(EntityUid actionId, EntityUid newContainer, EntityUid newAttached, ActionComponent? action = null, ActionsContainerComponent? container = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		Entity<ActionComponent>? action2 = _actions.GetAction(Entity<ActionComponent>.op_Implicit((actionId, action)));
		if (action2.HasValue)
		{
			Entity<ActionComponent> ent = action2.GetValueOrDefault();
			EntityUid? container2 = ent.Comp.Container;
			if ((!container2.HasValue || !(container2.GetValueOrDefault() == newContainer)) && AddAction(newContainer, Entity<ActionComponent>.op_Implicit(ent), ent.Comp, container))
			{
				_actions.AddActionDirect(Entity<ActionsComponent>.op_Implicit(newAttached), Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(ent), ent.Comp)));
			}
		}
	}

	public void TransferAllActionsWithNewAttached(EntityUid from, EntityUid to, EntityUid newAttached, ActionsContainerComponent? oldContainer = null, ActionsContainerComponent? newContainer = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ActionsContainerComponent>(from, ref oldContainer, true) && ((EntitySystem)this).Resolve<ActionsContainerComponent>(to, ref newContainer, true))
		{
			EntityUid[] array = ((BaseContainer)oldContainer.Container).ContainedEntities.ToArray();
			foreach (EntityUid action in array)
			{
				TransferActionWithNewAttached(action, to, newAttached, null, newContainer);
			}
		}
	}

	public bool AddAction(EntityUid uid, EntityUid actionId, ActionComponent? action = null, ActionsContainerComponent? comp = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		Entity<ActionComponent>? action2 = _actions.GetAction(Entity<ActionComponent>.op_Implicit((actionId, action)));
		if (action2.HasValue)
		{
			Entity<ActionComponent> ent = action2.GetValueOrDefault();
			if (ent.Comp.Container.HasValue)
			{
				RemoveAction(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(ent), Entity<ActionComponent>.op_Implicit(ent))));
			}
			if (comp == null)
			{
				comp = ((EntitySystem)this).EnsureComp<ActionsContainerComponent>(uid);
			}
			if (!_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(ent.Owner), (BaseContainer)(object)comp.Container, (TransformComponent)null, false))
			{
				((EntitySystem)this).Log.Error($"Failed to insert action {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<ActionComponent>.op_Implicit(ent), (MetaDataComponent)null)} into {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}");
				return false;
			}
			return true;
		}
		return false;
	}

	public void RemoveAction(Entity<ActionComponent?>? action, bool logMissing = true)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		Entity<ActionComponent>? action2 = _actions.GetAction(action, logMissing);
		if (!action2.HasValue)
		{
			return;
		}
		Entity<ActionComponent> ent = action2.GetValueOrDefault();
		if (!ent.Comp.Container.HasValue)
		{
			return;
		}
		_transform.DetachEntity(Entity<ActionComponent>.op_Implicit(ent), ((EntitySystem)this).Transform(Entity<ActionComponent>.op_Implicit(ent)));
		EntityUid? container = ent.Comp.Container;
		if (container.HasValue)
		{
			EntityUid container2 = container.GetValueOrDefault();
			if (((EntitySystem)this).Exists(container2))
			{
				((EntitySystem)this).Log.Error($"Failed to remove action {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<ActionComponent>.op_Implicit(ent), (MetaDataComponent)null)} from its container {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(container2))}?");
			}
			ent.Comp.Container = null;
			((EntitySystem)this).DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(ent), ent.Comp, "Container", (MetaDataComponent)null);
		}
		container = ent.Comp.AttachedEntity;
		if (container.HasValue)
		{
			EntityUid actions = container.GetValueOrDefault();
			_actions.RemoveAction(Entity<ActionsComponent>.op_Implicit(actions), Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(ent), Entity<ActionComponent>.op_Implicit(ent))));
		}
	}

	private void OnInit(EntityUid uid, ActionsContainerComponent component, ComponentInit args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		component.Container = _container.EnsureContainer<Container>(uid, "actions", (ContainerManagerComponent)null);
	}

	private void OnShutdown(EntityUid uid, ActionsContainerComponent component, ComponentShutdown args)
	{
		if (!_timing.ApplyingState || !((Component)component).NetSyncEnabled)
		{
			_container.ShutdownContainer((BaseContainer)(object)component.Container);
		}
	}

	private void OnEntityInserted(EntityUid uid, ActionsContainerComponent component, EntInsertedIntoContainerMessage args)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if (((ContainerModifiedMessage)args).Container.ID != "actions")
		{
			return;
		}
		Entity<ActionComponent>? action = _actions.GetAction(Entity<ActionComponent>.op_Implicit(((ContainerModifiedMessage)args).Entity));
		if (action.HasValue)
		{
			Entity<ActionComponent> action2 = action.GetValueOrDefault();
			EntityUid? container = action2.Comp.Container;
			if (!container.HasValue || container.GetValueOrDefault() != uid)
			{
				action2.Comp.Container = uid;
				((EntitySystem)this).DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(action2), action2.Comp, "Container", (MetaDataComponent)null);
			}
			ActionAddedEvent ev = new ActionAddedEvent(((ContainerModifiedMessage)args).Entity, Entity<ActionComponent>.op_Implicit(action2));
			((EntitySystem)this).RaiseLocalEvent<ActionAddedEvent>(uid, ref ev, false);
		}
	}

	private void OnEntityRemoved(EntityUid uid, ActionsContainerComponent component, EntRemovedFromContainerMessage args)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		if (((ContainerModifiedMessage)args).Container.ID != "actions")
		{
			return;
		}
		Entity<ActionComponent>? action = _actions.GetAction(Entity<ActionComponent>.op_Implicit(((ContainerModifiedMessage)args).Entity), logError: false);
		if (action.HasValue)
		{
			Entity<ActionComponent> action2 = action.GetValueOrDefault();
			ActionRemovedEvent ev = new ActionRemovedEvent(((ContainerModifiedMessage)args).Entity, Entity<ActionComponent>.op_Implicit(action2));
			((EntitySystem)this).RaiseLocalEvent<ActionRemovedEvent>(uid, ref ev, false);
			if (action2.Comp.Container.HasValue)
			{
				action2.Comp.Container = null;
				((EntitySystem)this).DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(action2), action2.Comp, "Container", (MetaDataComponent)null);
			}
		}
	}

	private void OnActionAdded(EntityUid uid, ActionsContainerComponent component, ActionAddedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		MindComponent mindComp = default(MindComponent);
		if (((EntitySystem)this).TryComp<MindComponent>(uid, ref mindComp) && mindComp.OwnedEntity.HasValue && ((EntitySystem)this).HasComp<ActionsContainerComponent>(mindComp.OwnedEntity.Value))
		{
			_actions.GrantContainedAction(Entity<ActionsComponent>.op_Implicit(mindComp.OwnedEntity.Value), Entity<ActionsContainerComponent>.op_Implicit(uid), args.Action);
		}
	}
}
