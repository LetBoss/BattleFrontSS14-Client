using System;
using System.Linq;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Item;
using Content.Shared.Storage.Components;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;

namespace Content.Shared.Storage.EntitySystems;

public sealed class BinSystem : EntitySystem
{
	[Dependency]
	private INetManager _net;

	[Dependency]
	private ISharedAdminLogManager _admin;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<BinComponent, ComponentStartup>((ComponentEventHandler<BinComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BinComponent, MapInitEvent>((ComponentEventHandler<BinComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BinComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<BinComponent, EntInsertedIntoContainerMessage>)OnEntInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BinComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<BinComponent, EntRemovedFromContainerMessage>)OnEntRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BinComponent, InteractHandEvent>((ComponentEventHandler<BinComponent, InteractHandEvent>)OnInteractHand, new Type[1] { typeof(SharedItemSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BinComponent, AfterInteractUsingEvent>((ComponentEventHandler<BinComponent, AfterInteractUsingEvent>)OnAfterInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BinComponent, GetVerbsEvent<AlternativeVerb>>((ComponentEventHandler<BinComponent, GetVerbsEvent<AlternativeVerb>>)OnAltInteractHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BinComponent, ExaminedEvent>((ComponentEventHandler<BinComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
	}

	private void OnExamined(EntityUid uid, BinComponent component, ExaminedEvent args)
	{
		args.PushText(base.Loc.GetString("bin-component-on-examine-text", (ValueTuple<string, object>)("count", component.Items.Count)));
	}

	private void OnStartup(EntityUid uid, BinComponent component, ComponentStartup args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		component.ItemContainer = _container.EnsureContainer<Container>(uid, component.ContainerId, (ContainerManagerComponent)null);
	}

	private void OnMapInit(EntityUid uid, BinComponent component, MapInitEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TransformComponent xform = ((EntitySystem)this).Transform(uid);
		foreach (EntProtoId id in component.InitialContents)
		{
			EntityUid ent = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(id), xform.Coordinates);
			if (!TryInsertIntoBin(uid, ent, component))
			{
				((EntitySystem)this).Log.Error($"Entity {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(ent))} was unable to be initialized into bin {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}");
				break;
			}
		}
	}

	private void OnEntInserted(Entity<BinComponent> ent, ref EntInsertedIntoContainerMessage args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (!(((ContainerModifiedMessage)args).Container.ID != ent.Comp.ContainerId))
		{
			ent.Comp.Items.Add(((ContainerModifiedMessage)args).Entity);
		}
	}

	private void OnEntRemoved(Entity<BinComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (!(((ContainerModifiedMessage)args).Container.ID != ent.Comp.ContainerId))
		{
			ent.Comp.Items.Remove(((ContainerModifiedMessage)args).Entity);
		}
	}

	private void OnInteractHand(EntityUid uid, BinComponent component, InteractHandEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			EntityUid? toGrab = component.Items.LastOrDefault();
			if (TryRemoveFromBin(uid, toGrab, component))
			{
				_hands.TryPickupAnyHand(args.User, toGrab.Value);
				ISharedAdminLogManager admin = _admin;
				LogStringHandler handler = new LogStringHandler(20, 3);
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "player", "ToPrettyString(uid)");
				handler.AppendLiteral(" removed ");
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(toGrab.Value)), "ToPrettyString(toGrab.Value)");
				handler.AppendLiteral(" from bin ");
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "ToPrettyString(uid)");
				handler.AppendLiteral(".");
				admin.Add(LogType.Pickup, LogImpact.Low, ref handler);
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
	}

	private void OnAltInteractHand(EntityUid uid, BinComponent component, GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (args.Using.HasValue)
		{
			bool canReach = args.CanAccess && args.CanInteract;
			InsertIntoBin(args.User, args.Target, args.Using.Value, component, handled: false, canReach);
		}
	}

	private void OnAfterInteractUsing(EntityUid uid, BinComponent component, AfterInteractUsingEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		InsertIntoBin(args.User, uid, args.Used, component, ((HandledEntityEventArgs)args).Handled, args.CanReach);
		((HandledEntityEventArgs)args).Handled = true;
	}

	private void InsertIntoBin(EntityUid user, EntityUid target, EntityUid itemInHand, BinComponent component, bool handled, bool canReach)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if (!handled && canReach && TryInsertIntoBin(target, itemInHand, component))
		{
			ISharedAdminLogManager admin = _admin;
			LogStringHandler handler = new LogStringHandler(21, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "player", "ToPrettyString(target)");
			handler.AppendLiteral(" inserted ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "ToPrettyString(user)");
			handler.AppendLiteral(" into bin ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "ToPrettyString(target)");
			handler.AppendLiteral(".");
			admin.Add(LogType.Pickup, LogImpact.Low, ref handler);
		}
	}

	public bool TryInsertIntoBin(EntityUid uid, EntityUid toInsert, BinComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BinComponent>(uid, ref component, true))
		{
			return false;
		}
		if (component.Items.Count >= component.MaxItems)
		{
			return false;
		}
		if (_whitelistSystem.IsWhitelistFail(component.Whitelist, toInsert))
		{
			return false;
		}
		_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(toInsert), (BaseContainer)(object)component.ItemContainer, (TransformComponent)null, false);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		return true;
	}

	public bool TryRemoveFromBin(EntityUid uid, EntityUid? toRemove, BinComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BinComponent>(uid, ref component, true))
		{
			return false;
		}
		if (component.Items.Count == 0)
		{
			return false;
		}
		if (toRemove.HasValue)
		{
			EntityUid? val = toRemove;
			EntityUid val2 = component.Items.LastOrDefault();
			if (val.HasValue && !(val.GetValueOrDefault() != val2))
			{
				if (!_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(toRemove.Value), (BaseContainer)(object)component.ItemContainer, true, false, (EntityCoordinates?)null, (Angle?)null))
				{
					return false;
				}
				component.Items.Remove(toRemove.Value);
				((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
				return true;
			}
		}
		return false;
	}
}
