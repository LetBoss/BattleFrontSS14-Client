using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Dropship.AttachmentPoint;
using Content.Shared._RMC14.Dropship.Utility.Components;
using Content.Shared._RMC14.Dropship.Weapon;
using Content.Shared._RMC14.Evacuation;
using Content.Shared._RMC14.Marines.Announce;
using Content.Shared._RMC14.Rules;
using Content.Shared._RMC14.Thunderdome;
using Content.Shared._RMC14.Tracker;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Maturing;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.GameTicking;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Shuttles.Components;
using Content.Shared.Shuttles.Systems;
using Content.Shared.UserInterface;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Dropship;

public abstract class SharedDropshipSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLog;

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedGameTicker _gameTicker;

	[Dependency]
	private SharedMarineAnnounceSystem _marineAnnounce;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	[Dependency]
	private IGameTiming _timing;

	private TimeSpan _dropshipInitialDelay;

	private TimeSpan _hijackInitialDelay;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<DropshipComponent, MapInitEvent>((EntityEventRefHandler<DropshipComponent, MapInitEvent>)OnDropshipMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipNavigationComputerComponent, MapInitEvent>((EntityEventRefHandler<DropshipNavigationComputerComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipNavigationComputerComponent, ActivatableUIOpenAttemptEvent>((EntityEventRefHandler<DropshipNavigationComputerComponent, ActivatableUIOpenAttemptEvent>)OnUIOpenAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipNavigationComputerComponent, AfterActivatableUIOpenEvent>((EntityEventRefHandler<DropshipNavigationComputerComponent, AfterActivatableUIOpenEvent>)OnNavigationOpen, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipTerminalComponent, ActivateInWorldEvent>((EntityEventRefHandler<DropshipTerminalComponent, ActivateInWorldEvent>)OnDropshipTerminalActivateInWorld, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipWeaponPointComponent, MapInitEvent>((EntityEventRefHandler<DropshipWeaponPointComponent, MapInitEvent>)OnAttachmentPointMapInit<DropshipWeaponPointComponent, MapInitEvent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipWeaponPointComponent, EntityTerminatingEvent>((EntityEventRefHandler<DropshipWeaponPointComponent, EntityTerminatingEvent>)OnAttachmentPointRemove<DropshipWeaponPointComponent, EntityTerminatingEvent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipWeaponPointComponent, ExaminedEvent>((EntityEventRefHandler<DropshipWeaponPointComponent, ExaminedEvent>)OnAttachmentExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipWeaponPointComponent, InteractHandEvent>((EntityEventRefHandler<DropshipWeaponPointComponent, InteractHandEvent>)OnInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipUtilityPointComponent, MapInitEvent>((EntityEventRefHandler<DropshipUtilityPointComponent, MapInitEvent>)OnAttachmentPointMapInit<DropshipUtilityPointComponent, MapInitEvent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipUtilityPointComponent, EntityTerminatingEvent>((EntityEventRefHandler<DropshipUtilityPointComponent, EntityTerminatingEvent>)OnAttachmentPointRemove<DropshipUtilityPointComponent, EntityTerminatingEvent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipEnginePointComponent, MapInitEvent>((EntityEventRefHandler<DropshipEnginePointComponent, MapInitEvent>)OnAttachmentPointMapInit<DropshipEnginePointComponent, MapInitEvent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipEnginePointComponent, EntityTerminatingEvent>((EntityEventRefHandler<DropshipEnginePointComponent, EntityTerminatingEvent>)OnAttachmentPointRemove<DropshipEnginePointComponent, EntityTerminatingEvent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipEnginePointComponent, ExaminedEvent>((EntityEventRefHandler<DropshipEnginePointComponent, ExaminedEvent>)OnEngineExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipElectronicSystemPointComponent, MapInitEvent>((EntityEventRefHandler<DropshipElectronicSystemPointComponent, MapInitEvent>)OnAttachmentPointMapInit<DropshipElectronicSystemPointComponent, MapInitEvent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipElectronicSystemPointComponent, EntityTerminatingEvent>((EntityEventRefHandler<DropshipElectronicSystemPointComponent, EntityTerminatingEvent>)OnAttachmentPointRemove<DropshipElectronicSystemPointComponent, EntityTerminatingEvent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipElectronicSystemPointComponent, ExaminedEvent>((EntityEventRefHandler<DropshipElectronicSystemPointComponent, ExaminedEvent>)OnElectronicSystemExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipElectronicSystemPointComponent, InteractHandEvent>((EntityEventRefHandler<DropshipElectronicSystemPointComponent, InteractHandEvent>)OnInteract, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<DropshipNavigationComputerComponent>(((EntitySystem)this).Subs, (object)DropshipNavigationUiKey.Key, (BuiEventSubscriber<DropshipNavigationComputerComponent>)delegate(Subscriber<DropshipNavigationComputerComponent> subs)
		{
			subs.Event<DropshipNavigationLaunchMsg>((EntityEventRefHandler<DropshipNavigationComputerComponent, DropshipNavigationLaunchMsg>)OnDropshipNavigationLaunchMsg);
			subs.Event<DropshipNavigationCancelMsg>((EntityEventRefHandler<DropshipNavigationComputerComponent, DropshipNavigationCancelMsg>)OnDropshipNavigationCancelMsg);
		});
		BoundUserInterfaceRegisterExt.BuiEvents<DropshipNavigationComputerComponent>(((EntitySystem)this).Subs, (object)DropshipHijackerUiKey.Key, (BuiEventSubscriber<DropshipNavigationComputerComponent>)delegate(Subscriber<DropshipNavigationComputerComponent> subs)
		{
			subs.Event<DropshipHijackerDestinationChosenBuiMsg>((EntityEventRefHandler<DropshipNavigationComputerComponent, DropshipHijackerDestinationChosenBuiMsg>)OnHijackerDestinationChosenMsg);
		});
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, RMCCVars.RMCDropshipInitialDelayMinutes, (Action<float>)delegate(float v)
		{
			_dropshipInitialDelay = TimeSpan.FromMinutes(v);
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCDropshipHijackInitialDelayMinutes, (Action<int>)delegate(int v)
		{
			_hijackInitialDelay = TimeSpan.FromMinutes(v);
		}, true);
	}

	private void OnDropshipMapInit(Entity<DropshipComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		TransformChildrenEnumerator children = ((EntitySystem)this).Transform(Entity<DropshipComponent>.op_Implicit(ent)).ChildEnumerator;
		EntityUid uid = default(EntityUid);
		while (((TransformChildrenEnumerator)(ref children)).MoveNext(ref uid))
		{
			if (!((EntitySystem)this).TerminatingOrDeleted(uid, (MetaDataComponent)null) && (((EntitySystem)this).HasComp<DropshipWeaponPointComponent>(uid) || ((EntitySystem)this).HasComp<DropshipEnginePointComponent>(uid) || ((EntitySystem)this).HasComp<DropshipUtilityPointComponent>(uid) || ((EntitySystem)this).HasComp<DropshipElectronicSystemPointComponent>(uid)))
			{
				ent.Comp.AttachmentPoints.Add(uid);
			}
		}
		DropshipMapInitEvent ev = default(DropshipMapInitEvent);
		((EntitySystem)this).RaiseLocalEvent<DropshipMapInitEvent>(Entity<DropshipComponent>.op_Implicit(ent), ref ev, false);
	}

	private void OnMapInit(Entity<DropshipNavigationComputerComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		EntityUid parent = ((EntitySystem)this).Transform(Entity<DropshipNavigationComputerComponent>.op_Implicit(ent)).ParentUid;
		if (((EntityUid)(ref parent)).Valid && IsShuttle(parent))
		{
			((EntitySystem)this).EnsureComp<DropshipComponent>(parent);
		}
	}

	private void OnUIOpenAttempt(Entity<DropshipNavigationComputerComponent> ent, ref ActivatableUIOpenAttemptEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled)
		{
			TransformComponent xform = ((EntitySystem)this).Transform(Entity<DropshipNavigationComputerComponent>.op_Implicit(ent));
			DropshipComponent dropship = default(DropshipComponent);
			if (((EntitySystem)this).TryComp<DropshipComponent>(xform.ParentUid, ref dropship) && dropship.Crashed)
			{
				((CancellableEntityEventArgs)args).Cancel();
			}
			else if (!TryDropshipLaunchPopup(Entity<DropshipNavigationComputerComponent>.op_Implicit(ent), args.User, predicted: true))
			{
				((CancellableEntityEventArgs)args).Cancel();
			}
		}
	}

	private void OnNavigationOpen(Entity<DropshipNavigationComputerComponent> ent, ref AfterActivatableUIOpenEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		RefreshUI(ent);
	}

	private void OnDropshipTerminalActivateInWorld(Entity<DropshipTerminalComponent> ent, ref ActivateInWorldEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		EntityUid user = args.User;
		if (!((EntitySystem)this).HasComp<XenoComponent>(user))
		{
			_popup.PopupEntity("This terminal doesn't seem to work yet... Maybe you should ask High Command?", user, user, PopupType.MediumCaution);
		}
		else if (!((EntitySystem)this).HasComp<DropshipHijackerComponent>(user))
		{
			_popup.PopupEntity("You stare cluelessly at the " + ((EntitySystem)this).Name(ent.Owner, (MetaDataComponent)null), user, user);
		}
		else
		{
			if (!TryDropshipLaunchPopup(Entity<DropshipTerminalComponent>.op_Implicit(ent), user, predicted: false) || !TryDropshipHijackPopup(Entity<DropshipTerminalComponent>.op_Implicit(ent), Entity<DropshipHijackerComponent>.op_Implicit(user), predicted: false))
			{
				return;
			}
			TransformComponent userTransform = ((EntitySystem)this).Transform(user);
			Entity<DropshipDestinationComponent, TransformComponent>? closestDestination = null;
			EntityQueryEnumerator<DropshipDestinationComponent, TransformComponent> destinations = ((EntitySystem)this).EntityQueryEnumerator<DropshipDestinationComponent, TransformComponent>();
			EntityUid uid = default(EntityUid);
			DropshipDestinationComponent destination = default(DropshipDestinationComponent);
			TransformComponent xform = default(TransformComponent);
			float distance = default(float);
			float oldDistance = default(float);
			while (destinations.MoveNext(ref uid, ref destination, ref xform))
			{
				if (xform.MapID != userTransform.MapID)
				{
					continue;
				}
				if (!closestDestination.HasValue)
				{
					closestDestination = Entity<DropshipDestinationComponent, TransformComponent>.op_Implicit((uid, destination, xform));
					continue;
				}
				EntityCoordinates coordinates = userTransform.Coordinates;
				if (((EntityCoordinates)(ref coordinates)).TryDistance((IEntityManager)(object)base.EntityManager, xform.Coordinates, ref distance))
				{
					coordinates = userTransform.Coordinates;
					if (((EntityCoordinates)(ref coordinates)).TryDistance((IEntityManager)(object)base.EntityManager, closestDestination.Value.Comp2.Coordinates, ref oldDistance) && distance < oldDistance)
					{
						closestDestination = Entity<DropshipDestinationComponent, TransformComponent>.op_Implicit((uid, destination, xform));
					}
				}
			}
			if (!closestDestination.HasValue)
			{
				_popup.PopupEntity("There are no dropship destinations near you!", user, user, PopupType.MediumCaution);
				return;
			}
			if (closestDestination.Value.Comp1.Ship.HasValue)
			{
				_popup.PopupEntity("There's already a dropship coming here!", user, user, PopupType.MediumCaution);
				return;
			}
			if (((EntitySystem)this).Count<PrimaryLandingZoneComponent>() > 0)
			{
				Entity<DropshipDestinationComponent, TransformComponent>? val = closestDestination;
				if (!((EntitySystem)this).HasComp<PrimaryLandingZoneComponent>(val.HasValue ? new EntityUid?(Entity<DropshipDestinationComponent, TransformComponent>.op_Implicit(val.GetValueOrDefault())) : ((EntityUid?)null)))
				{
					_popup.PopupEntity("The shuttle isn't responding to prompts, it looks like this isn't the primary shuttle.", user, user, PopupType.MediumCaution);
					return;
				}
			}
			EntityQueryEnumerator<DropshipComponent, TransformComponent> dropships = ((EntitySystem)this).EntityQueryEnumerator<DropshipComponent, TransformComponent>();
			EntityUid uid2 = default(EntityUid);
			DropshipComponent dropship = default(DropshipComponent);
			TransformComponent xform2 = default(TransformComponent);
			EntityUid computerId = default(EntityUid);
			DropshipNavigationComputerComponent computer = default(DropshipNavigationComputerComponent);
			while (dropships.MoveNext(ref uid2, ref dropship, ref xform2))
			{
				if (dropship.Crashed || IsInFTL(uid2) || ((EntitySystem)this).HasComp<ThunderdomeMapComponent>(xform2.MapUid))
				{
					continue;
				}
				EntityQueryEnumerator<DropshipNavigationComputerComponent> computerQuery = ((EntitySystem)this).EntityQueryEnumerator<DropshipNavigationComputerComponent>();
				while (computerQuery.MoveNext(ref computerId, ref computer))
				{
					if (computer.Hijackable)
					{
						EntityUid? gridUid = ((EntitySystem)this).Transform(computerId).GridUid;
						EntityUid val2 = uid2;
						if (gridUid.HasValue && gridUid.GetValueOrDefault() == val2 && FlyTo(Entity<DropshipNavigationComputerComponent>.op_Implicit((computerId, computer)), Entity<DropshipDestinationComponent, TransformComponent>.op_Implicit(closestDestination.Value), user))
						{
							_popup.PopupEntity("You call down one of the dropships to your location", user, user, PopupType.LargeCaution);
							return;
						}
					}
				}
			}
			_popup.PopupEntity("There are no available dropships! Wait a moment.", user, user, PopupType.LargeCaution);
		}
	}

	private void OnAttachmentPointMapInit<TComp, TEvent>(Entity<TComp> ent, ref TEvent args) where TComp : IComponent
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient && TryGetGridDropship(Entity<TComp>.op_Implicit(ent), out Entity<DropshipComponent> dropship))
		{
			dropship.Comp.AttachmentPoints.Add(Entity<TComp>.op_Implicit(ent));
			((EntitySystem)this).Dirty<DropshipComponent>(dropship, (MetaDataComponent)null);
		}
	}

	private void OnAttachmentPointRemove<TComp, TEvent>(Entity<TComp> ent, ref TEvent args) where TComp : IComponent
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetGridDropship(Entity<TComp>.op_Implicit(ent), out Entity<DropshipComponent> dropship))
		{
			dropship.Comp.AttachmentPoints.Remove(Entity<TComp>.op_Implicit(ent));
			((EntitySystem)this).Dirty<DropshipComponent>(dropship, (MetaDataComponent)null);
		}
	}

	private void OnAttachmentExamined(Entity<DropshipWeaponPointComponent> ent, ref ExaminedEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("DropshipWeaponPointComponent"))
		{
			if (TryGetAttachmentContained(Entity<DropshipWeaponPointComponent>.op_Implicit(ent), ent.Comp.WeaponContainerSlotId, out var weapon))
			{
				args.PushText(base.Loc.GetString("rmc-dropship-attached", (ValueTuple<string, object>)("attachment", weapon)));
			}
			if (TryGetAttachmentContained(Entity<DropshipWeaponPointComponent>.op_Implicit(ent), ent.Comp.AmmoContainerSlotId, out var ammo))
			{
				args.PushText(base.Loc.GetString("rmc-dropship-weapons-point-ammo", (ValueTuple<string, object>)("ammo", ammo)));
				DropshipAmmoComponent ammoComp = default(DropshipAmmoComponent);
				if (((EntitySystem)this).TryComp<DropshipAmmoComponent>(ammo, ref ammoComp))
				{
					args.PushText(base.Loc.GetString("rmc-dropship-weapons-rounds-left", (ValueTuple<string, object>)("current", ammoComp.Rounds), (ValueTuple<string, object>)("max", ammoComp.MaxRounds)));
				}
			}
		}
	}

	private void OnEngineExamined(Entity<DropshipEnginePointComponent> ent, ref ExaminedEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("DropshipWeaponPointComponent"))
		{
			if (TryGetAttachmentContained(Entity<DropshipEnginePointComponent>.op_Implicit(ent), ent.Comp.ContainerId, out var attachment))
			{
				args.PushText(base.Loc.GetString("rmc-dropship-attached", (ValueTuple<string, object>)("attachment", attachment)));
			}
		}
	}

	private void OnElectronicSystemExamined(Entity<DropshipElectronicSystemPointComponent> ent, ref ExaminedEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("DropshipWeaponPointComponent"))
		{
			if (TryGetAttachmentContained(Entity<DropshipElectronicSystemPointComponent>.op_Implicit(ent), ent.Comp.ContainerId, out var attachment))
			{
				args.PushText(base.Loc.GetString("rmc-dropship-attached", (ValueTuple<string, object>)("attachment", attachment)));
			}
		}
	}

	private void OnDropshipNavigationLaunchMsg(Entity<DropshipNavigationComputerComponent> ent, ref DropshipNavigationLaunchMsg args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = ((BaseBoundUserInterfaceEvent)args).Actor;
		EntityUid? destination = default(EntityUid?);
		if (!((EntitySystem)this).TryGetEntity(args.Target, ref destination))
		{
			((EntitySystem)this).Log.Warning($"{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user))} tried to launch to invalid dropship destination {args.Target}");
		}
		else if (!((EntitySystem)this).HasComp<DropshipDestinationComponent>(destination))
		{
			((EntitySystem)this).Log.Warning($"{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor))} tried to launch to invalid dropship destination {((EntitySystem)this).ToPrettyString(destination, (MetaDataComponent)null)}");
		}
		else
		{
			FlyTo(ent, destination.Value, user);
		}
	}

	private void OnDropshipNavigationCancelMsg(Entity<DropshipNavigationComputerComponent> ent, ref DropshipNavigationCancelMsg args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? grid = _transform.GetGrid(Entity<TransformComponent>.op_Implicit((ent.Owner, ((EntitySystem)this).Transform(ent.Owner))));
		FTLComponent ftl = default(FTLComponent);
		DropshipComponent dropship = default(DropshipComponent);
		if (((EntitySystem)this).TryComp<FTLComponent>(grid, ref ftl) && ((EntitySystem)this).TryComp<DropshipComponent>(grid, ref dropship))
		{
			EntityUid? destination = dropship.Destination;
			EntityUid? departureLocation = dropship.DepartureLocation;
			if (destination.HasValue == departureLocation.HasValue && (!destination.HasValue || !(destination.GetValueOrDefault() != departureLocation.GetValueOrDefault())) && !(_timing.CurTime + dropship.CancelFlightTime >= ftl.StateTime.End))
			{
				ftl.StateTime.End = _timing.CurTime + dropship.CancelFlightTime;
				((EntitySystem)this).Dirty(grid.Value, (IComponent)(object)dropship, (MetaDataComponent)null);
				RefreshUI();
			}
		}
	}

	private void OnHijackerDestinationChosenMsg(Entity<DropshipNavigationComputerComponent> ent, ref DropshipHijackerDestinationChosenBuiMsg args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)DropshipHijackerUiKey.Key, (EntityUid?)((BaseBoundUserInterfaceEvent)args).Actor, false);
		EntityUid? destination = default(EntityUid?);
		TransformComponent xform = default(TransformComponent);
		if (!((EntitySystem)this).TryGetEntity(args.Destination, ref destination))
		{
			((EntitySystem)this).Log.Warning($"{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor))} tried to hijack to invalid destination");
		}
		else if (!((EntitySystem)this).HasComp<DropshipHijackDestinationComponent>(destination))
		{
			((EntitySystem)this).Log.Warning($"{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor))} tried to hijack to invalid destination {((EntitySystem)this).ToPrettyString(destination, (MetaDataComponent)null)}");
		}
		else if (FlyTo(ent, destination.Value, ((BaseBoundUserInterfaceEvent)args).Actor, hijack: true) && ((EntitySystem)this).TryComp(Entity<DropshipNavigationComputerComponent>.op_Implicit(ent), ref xform))
		{
			EntityUid parentUid = xform.ParentUid;
			if (((EntityUid)(ref parentUid)).Valid)
			{
				DropshipComponent dropship = ((EntitySystem)this).EnsureComp<DropshipComponent>(xform.ParentUid);
				dropship.Crashed = true;
				((EntitySystem)this).Dirty(xform.ParentUid, (IComponent)(object)dropship, (MetaDataComponent)null);
				DropshipHijackStartEvent ev = new DropshipHijackStartEvent(xform.ParentUid);
				((EntitySystem)this).RaiseLocalEvent<DropshipHijackStartEvent>(ref ev);
			}
		}
	}

	private void OnInteract(Entity<DropshipWeaponPointComponent> ent, ref InteractHandEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		ContainerSlot slot = _container.EnsureContainer<ContainerSlot>(Entity<DropshipWeaponPointComponent>.op_Implicit(ent), ent.Comp.WeaponContainerSlotId, (ContainerManagerComponent)null);
		RelayInteractToContained(slot, ref args);
	}

	private void OnInteract(Entity<DropshipElectronicSystemPointComponent> ent, ref InteractHandEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		ContainerSlot slot = _container.EnsureContainer<ContainerSlot>(Entity<DropshipElectronicSystemPointComponent>.op_Implicit(ent), ent.Comp.ContainerId, (ContainerManagerComponent)null);
		RelayInteractToContained(slot, ref args);
	}

	private void RelayInteractToContained(ContainerSlot slot, ref InteractHandEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? deployer = slot.ContainedEntity;
		if (((EntitySystem)this).HasComp<RMCEquipmentDeployerComponent>(deployer))
		{
			InteractHandEvent ev = new InteractHandEvent(args.User, args.Target);
			((EntitySystem)this).RaiseLocalEvent<InteractHandEvent>(deployer.Value, ev, false);
			((HandledEntityEventArgs)args).Handled = ((HandledEntityEventArgs)ev).Handled;
		}
	}

	public virtual bool FlyTo(Entity<DropshipNavigationComputerComponent> computer, EntityUid destination, EntityUid? user, bool hijack = false, float? startupTime = null, float? hyperspaceTime = null, bool offset = false)
	{
		return false;
	}

	protected virtual void RefreshUI()
	{
	}

	protected virtual void RefreshUI(Entity<DropshipNavigationComputerComponent> computer)
	{
	}

	protected virtual bool IsShuttle(EntityUid dropship)
	{
		return false;
	}

	protected virtual bool IsInFTL(EntityUid dropship)
	{
		return false;
	}

	private bool TryDropshipLaunchPopup(EntityUid computer, EntityUid user, bool predicted)
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan roundDuration = _gameTicker.RoundDuration();
		if (roundDuration < _dropshipInitialDelay)
		{
			int minutesLeft = Math.Max(1, (int)(_dropshipInitialDelay - roundDuration).TotalMinutes);
			string msg = base.Loc.GetString("rmc-dropship-pre-flight-fueling", (ValueTuple<string, object>)("minutes", minutesLeft));
			if (predicted)
			{
				_popup.PopupClient(msg, computer, user, PopupType.MediumCaution);
			}
			else
			{
				_popup.PopupEntity(msg, computer, user, PopupType.MediumCaution);
			}
			return false;
		}
		return true;
	}

	protected bool TryDropshipHijackPopup(EntityUid computer, Entity<DropshipHijackerComponent?> user, bool predicted)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan roundDuration = _gameTicker.RoundDuration();
		if (((EntitySystem)this).HasComp<DropshipHijackerComponent>(Entity<DropshipHijackerComponent>.op_Implicit(user)) && roundDuration < _hijackInitialDelay)
		{
			int minutesLeft = Math.Max(1, (int)(_hijackInitialDelay - roundDuration).TotalMinutes);
			string msg = base.Loc.GetString("rmc-dropship-pre-hijack", (ValueTuple<string, object>)("minutes", minutesLeft));
			if (predicted)
			{
				_popup.PopupClient(msg, computer, Entity<DropshipHijackerComponent>.op_Implicit(user), PopupType.MediumCaution);
			}
			else
			{
				_popup.PopupEntity(msg, computer, Entity<DropshipHijackerComponent>.op_Implicit(user), PopupType.MediumCaution);
			}
			return false;
		}
		EntityUid? map = _transform.GetMap(Entity<TransformComponent>.op_Implicit(user.Owner));
		EvacuationProgressComponent evacuation = default(EvacuationProgressComponent);
		if ((((EntitySystem)this).HasComp<XenoMaturingComponent>(Entity<DropshipHijackerComponent>.op_Implicit(user)) && !((EntitySystem)this).HasComp<RMCPlanetComponent>(map)) || (((EntitySystem)this).TryComp<EvacuationProgressComponent>(map, ref evacuation) && evacuation.DropShipCrashed))
		{
			string msg2 = base.Loc.GetString("rmc-dropship-invalid-hijack");
			if (predicted)
			{
				_popup.PopupClient(msg2, computer, Entity<DropshipHijackerComponent>.op_Implicit(user), PopupType.MediumCaution);
			}
			else
			{
				_popup.PopupEntity(msg2, computer, Entity<DropshipHijackerComponent>.op_Implicit(user), PopupType.MediumCaution);
			}
			return false;
		}
		return true;
	}

	public bool TryDesignatePrimaryLZ(EntityUid actor, EntityUid lz)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<DropshipDestinationComponent>(lz))
		{
			((EntitySystem)this).Log.Warning($"{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(actor))} tried to designate as primary LZ entity {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(lz))} with no {"DropshipDestinationComponent"}!");
			return false;
		}
		if (((EntitySystem)this).Count<PrimaryLandingZoneComponent>() > 0)
		{
			((EntitySystem)this).Log.Warning($"{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(actor))} tried to designate as primary LZ entity {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(lz))} when one already exists!");
			return false;
		}
		if (!((EntitySystem)this).HasComp<RMCPlanetComponent>(_transform.GetGrid(Entity<TransformComponent>.op_Implicit(lz))) && !((EntitySystem)this).HasComp<RMCPlanetComponent>(_transform.GetMap(Entity<TransformComponent>.op_Implicit(lz))))
		{
			((EntitySystem)this).Log.Warning($"{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(actor))} tried to designate entity {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(lz))} on the warship as primary LZ!");
			return false;
		}
		if (GetPrimaryLZCandidates().All<Entity<MetaDataComponent>>((Entity<MetaDataComponent> candidate) => candidate.Owner != lz))
		{
			((EntitySystem)this).Log.Warning($"{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(actor))} tried to designate invalid primary LZ entity {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(lz))}!");
			return false;
		}
		ISharedAdminLogManager adminLog = _adminLog;
		LogStringHandler handler = new LogStringHandler(36, 2);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(actor)), "player", "ToPrettyString(actor)");
		handler.AppendLiteral(" designated ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(lz)), "lz", "ToPrettyString(lz)");
		handler.AppendLiteral(" as primary landing zone");
		adminLog.Add(LogType.RMCPrimaryLZ, ref handler);
		((EntitySystem)this).EnsureComp<PrimaryLandingZoneComponent>(lz);
		((EntitySystem)this).EnsureComp<RMCTrackableComponent>(lz);
		RefreshUI();
		string message = base.Loc.GetString("rmc-announcement-ares-lz-designated", (ValueTuple<string, object>)("name", ((EntitySystem)this).Name(lz, (MetaDataComponent)null)));
		_marineAnnounce.AnnounceARESStaging(actor, message);
		return true;
	}

	public IEnumerable<Entity<MetaDataComponent>> GetPrimaryLZCandidates()
	{
		if (((EntitySystem)this).Count<PrimaryLandingZoneComponent>() != 0)
		{
			yield break;
		}
		EntityQueryEnumerator<DropshipDestinationComponent, MetaDataComponent, TransformComponent> landingZoneQuery = ((EntitySystem)this).EntityQueryEnumerator<DropshipDestinationComponent, MetaDataComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		DropshipDestinationComponent dropshipDestinationComponent = default(DropshipDestinationComponent);
		MetaDataComponent metaData = default(MetaDataComponent);
		TransformComponent xform = default(TransformComponent);
		while (landingZoneQuery.MoveNext(ref uid, ref dropshipDestinationComponent, ref metaData, ref xform))
		{
			if (((EntitySystem)this).HasComp<RMCPlanetComponent>(xform.ParentUid) || ((EntitySystem)this).HasComp<RMCPlanetComponent>(xform.MapUid))
			{
				yield return Entity<MetaDataComponent>.op_Implicit((uid, metaData));
			}
		}
	}

	public bool TryGetGridDropship(EntityUid ent, out Entity<DropshipComponent> dropship)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = default(TransformComponent);
		if (((EntitySystem)this).TryComp(ent, ref xform))
		{
			EntityUid? gridUid = xform.GridUid;
			if (gridUid.HasValue)
			{
				EntityUid grid = gridUid.GetValueOrDefault();
				DropshipComponent dropshipComp = default(DropshipComponent);
				if (!((EntitySystem)this).TerminatingOrDeleted(grid, (MetaDataComponent)null) && ((EntitySystem)this).TryComp<DropshipComponent>(xform.GridUid, ref dropshipComp))
				{
					dropship = Entity<DropshipComponent>.op_Implicit((grid, dropshipComp));
					return true;
				}
			}
		}
		dropship = default(Entity<DropshipComponent>);
		return false;
	}

	public bool IsWeaponAttached(Entity<DropshipWeaponComponent?> weapon)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DropshipWeaponComponent>(Entity<DropshipWeaponComponent>.op_Implicit(weapon), ref weapon.Comp, false) || !TryGetGridDropship(Entity<DropshipWeaponComponent>.op_Implicit(weapon), out Entity<DropshipComponent> dropship))
		{
			return false;
		}
		BaseContainer container = default(BaseContainer);
		if (!_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(Entity<DropshipWeaponComponent>.op_Implicit(weapon), null)), ref container) || !dropship.Comp.AttachmentPoints.Contains(container.Owner))
		{
			return false;
		}
		return true;
	}

	public bool TryGetAttachmentContained(EntityUid point, string containerId, out EntityUid contained)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		contained = default(EntityUid);
		BaseContainer container = default(BaseContainer);
		if (!_container.TryGetContainer(point, containerId, ref container, (ContainerManagerComponent)null) || container.ContainedEntities.Count == 0)
		{
			return false;
		}
		contained = container.ContainedEntities[0];
		return true;
	}

	public bool IsInFlight(Entity<DropshipComponent?> dropship)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DropshipComponent>(Entity<DropshipComponent>.op_Implicit(dropship), ref dropship.Comp, false))
		{
			return false;
		}
		if (dropship.Comp.State != FTLState.Travelling)
		{
			return dropship.Comp.State == FTLState.Arriving;
		}
		return true;
	}

	public bool IsOnDropship(EntityUid entity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? grid = _transform.GetGrid(Entity<TransformComponent>.op_Implicit(entity));
		return ((EntitySystem)this).HasComp<DropshipComponent>(grid);
	}

	public bool IsOnDropship(EntityCoordinates coordinates)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? grid = _transform.GetGrid(coordinates);
		return ((EntitySystem)this).HasComp<DropshipComponent>(grid);
	}
}
